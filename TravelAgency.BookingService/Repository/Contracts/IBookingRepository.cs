using TravelAgency.Contracts.Entity;
using TravelAgency.Contracts.Repository.Contracts;

namespace TravelAgency.BookingService.Repository.Contracts;

public interface IBookingRepository : IEntityRepository<BookingEntity>
{
    Task<IReadOnlyList<BookingEntity>> GetByCustomerAsync(string customerName, CancellationToken ct = default);
    Task<IReadOnlyList<BookingEntity>> GetPendingAsync(CancellationToken ct = default);
}
