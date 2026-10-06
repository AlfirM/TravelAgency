using MassTransit.SqlTransport;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Contracts.Repository;
using TravelAgency.NotificationService.Entity;
using TravelAgency.NotificationService.Repository.Contracts;

namespace TravelAgency.NotificationService.Repository;

public class NotificationRepository : EntityRepository<NotificationLog>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<NotificationLog>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(n => n.BookingId == bookingId)
            .ToListAsync(ct);
}
