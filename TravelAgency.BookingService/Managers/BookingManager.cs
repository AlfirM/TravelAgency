// TravelAgency.BookingService/Services/BookingManager.cs
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Repository.Contracts;
using TravelAgency.Contracts;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Managers
{
    public class BookingManager : IBookingManager
    {
        private readonly IBookingRepository _repository;
        private readonly ILogger<BookingManager> _logger;

        public BookingManager(IBookingRepository repository, ILogger<BookingManager> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BookingEntity> CreateBookingAsync(
            string customerName, string destination, decimal price, CancellationToken ct = default)
        {
            var booking = new BookingEntity
            {
                CustomerName = customerName,
                Destination = destination,
                Price = price,
                BookingDate = DateTime.UtcNow,
                Status = "Pending"
            };

            // Repository.AddAsync сам вызывает SaveChangesAsync — это нормально:
            // outbox-сообщение будет сохранено в той же транзакции, что вызовет контроллер.
            await _repository.AddAsync(booking, ct);
            _logger.LogInformation("Бронирование сохранено: {BookingId}", booking.Id);
            return booking;
        }

        public Task<BookingEntity?> GetBookingAsync(Guid id, CancellationToken ct = default)
            => _repository.GetByIdAsync(id, ct);

        public Task<IReadOnlyList<BookingEntity>> GetAllBookingsAsync(CancellationToken ct = default)
            => _repository.GetAllAsync(ct);

        public async Task<bool> UpdateStatusAsync(Guid bookingId, string status, CancellationToken ct = default)
        {
            var booking = await _repository.GetByIdAsync(bookingId, ct);
            if (booking is null)
            {
                _logger.LogWarning("Бронирование {BookingId} не найдено", bookingId);
                return false;
            }

            booking.Status = status;
            await _repository.UpdateAsync(booking, ct);
            _logger.LogInformation("Статус {BookingId} изменён на {Status}", bookingId, status);
            return true;
        }
    }
}