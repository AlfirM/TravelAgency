using TravelAgency.Contracts.Repository.Contracts;
using TravelAgency.PaymentService.Entity;

namespace TravelAgency.PaymentService.Repository.Contracts;

public interface IPaymentRepository : IEntityRepository<PaymentRecord>
{
    Task<IReadOnlyList<PaymentRecord>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
}
