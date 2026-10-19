// TravelAgency.BookingService/Controllers/BookingsController.cs
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BookingService.Messaging;
using TravelAgency.BookingService.Models;
using TravelAgency.BookingService.Managers.Contracts;

namespace TravelAgency.BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingManager _bookingManager;
        private readonly IBookingEventPublisher _eventPublisher;
        private readonly BookingDbContext _db;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingManager bookingManager,
            IBookingEventPublisher eventPublisher,
            BookingDbContext db,
            ILogger<BookingsController> logger)
        {
            _bookingManager = bookingManager;
            _eventPublisher = eventPublisher;
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookingRequest request, CancellationToken ct)
        {
            // Явная транзакция: данные + outbox-сообщение коммитятся атомарно
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                // 1. Сохраняем бронирование (SaveChanges внутри транзакции)
                var booking = await _bookingManager.CreateBookingAsync(
                    request.CustomerName, request.Destination, request.Price, ct);

                // 2. Ставим событие в outbox-буфер (в RabbitMQ ещё ничего не ушло)
                await _eventPublisher.PublishBookingPlacedAsync(booking, ct);

                // 3. Финальный SaveChanges — MassTransit запишет OutboxMessage
                await _db.SaveChangesAsync(ct);

                // 4. Коммит: и данные, и outbox-сообщение теперь надёжно сохранены
                await tx.CommitAsync(ct);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.Id },
                    booking.ToResponse());
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var booking = await _bookingManager.GetBookingAsync(id, ct);
            return booking is null
                ? NotFound(new { Message = $"Бронирование {id} не найдено" })
                : Ok(booking.ToResponse());
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var bookings = await _bookingManager.GetAllBookingsAsync(ct);
            return Ok(bookings.Select(b => b.ToResponse()));
        }

        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(
            Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var updated = await _bookingManager.UpdateStatusAsync(id, request.Status, ct);
                if (!updated)
                {
                    await tx.RollbackAsync(ct);
                    return NotFound(new { Message = $"Бронирование {id} не найдено" });
                }

                await _eventPublisher.PublishBookingStatusChangedAsync(id, request.Status, ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}