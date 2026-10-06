using TravelAgency.Contracts.Repository.Contracts;
using TravelAgency.NotificationService.Entity;

namespace TravelAgency.NotificationService.Repository.Contracts;

public interface INotificationRepository : IEntityRepository<NotificationLog>
{
    Task<IReadOnlyList<NotificationLog>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
}
