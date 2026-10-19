// TravelAgency.BookingService/Controllers/BookingsController.cs
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Models;

namespace TravelAgency.BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]           // → /api/bookings
    [Produces("application/json")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingManager _bookingManager;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingManager bookingManager,
            ILogger<BookingsController> logger)
        {
            _bookingManager = bookingManager;
            _logger = logger;
        }

        /// <summary>
        /// Создать новое бронирование.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] BookingRequest request,
            CancellationToken ct)
        {
            var booking = await _bookingManager.CreateBookingAsync(
                request.CustomerName, request.Destination, request.Price, ct);

            _logger.LogInformation("Создано бронирование {BookingId}", booking.Id);

            // 201 Created + Location: /api/bookings/{id}
            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                booking.ToResponse());
        }

        /// <summary>
        /// Получить бронирование по идентификатору.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var booking = await _bookingManager.GetBookingAsync(id, ct);
            if (booking is null)
                return NotFound(new { Message = $"Бронирование {id} не найдено" });

            return Ok(booking.ToResponse());
        }

        /// <summary>
        /// Получить все бронирования.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var bookings = await _bookingManager.GetAllBookingsAsync(ct);
            return Ok(bookings.Select(b => b.ToResponse()));
        }

        /// <summary>
        /// Обновить статус бронирования (например, вручную из админки).
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateStatusRequest request,
            CancellationToken ct)
        {
            var booking = await _bookingManager.GetBookingAsync(id, ct);
            if (booking is null)
                return NotFound(new { Message = $"Бронирование {id} не найдено" });

            await _bookingManager.UpdateStatusAsync(id, request.Status, ct);
            return NoContent();
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}