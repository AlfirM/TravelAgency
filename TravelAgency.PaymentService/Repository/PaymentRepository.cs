using Microsoft.EntityFrameworkCore;
using TravelAgency.Contracts.Repository;
using TravelAgency.PaymentService.Entity;
using TravelAgency.PaymentService.Repository.Contracts;

namespace TravelAgency.PaymentService.Repository;

public class PaymentRepository : EntityRepository<PaymentRecord>, IPaymentRepository
{
    public PaymentRepository(PaymentDbContext context) : base(context) { }

    public async Task<IReadOnlyList<PaymentRecord>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(p => p.BookingId == bookingId)
            .ToListAsync(ct);
}
