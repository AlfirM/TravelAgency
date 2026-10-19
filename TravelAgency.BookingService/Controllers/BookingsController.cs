// TravelAgency.BookingService/Controllers/BookingsController.cs
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Messaging;
using TravelAgency.BookingService.Models;

namespace TravelAgency.BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingManager _bookingManager;
        private readonly IBookingEventPublisher _eventPublisher;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingManager bookingManager,
            IBookingEventPublisher eventPublisher,
            ILogger<BookingsController> logger)
        {
            _bookingManager = bookingManager;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookingRequest request, CancellationToken ct)
        {
            // 1. Сохраняем в БД
            var booking = await _bookingManager.CreateBookingAsync(
                request.CustomerName, request.Destination, request.Price, ct);

            // 2. Публикуем событие
            await _eventPublisher.PublishBookingPlacedAsync(booking, ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                booking.ToResponse());
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
            var updated = await _bookingManager.UpdateStatusAsync(id, request.Status, ct);
            if (!updated)
                return NotFound(new { Message = $"Бронирование {id} не найдено" });

            await _eventPublisher.PublishBookingStatusChangedAsync(id, request.Status, ct);
            return NoContent();
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}