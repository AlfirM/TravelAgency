using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Managers.Contracts;

public interface IBookingManager
{
    Task<BookingEntity> CreateBookingAsync(string customerName, string destination, decimal price, CancellationToken ct = default);
    Task<BookingEntity?> GetBookingAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BookingEntity>> GetAllBookingsAsync(CancellationToken ct = default);
    Task UpdateStatusAsync(Guid bookingId, string status, CancellationToken ct = default);
}
