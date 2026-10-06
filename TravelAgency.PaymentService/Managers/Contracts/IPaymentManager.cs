using TravelAgency.PaymentService.Entity;

namespace TravelAgency.PaymentService.Managers.Contracts;

public interface IPaymentManager
{
    Task<bool> ProcessPaymentAsync(Guid bookingId, decimal amount, CancellationToken ct = default);
    Task<IReadOnlyList<PaymentRecord>> GetPaymentsByBookingAsync(Guid bookingId, CancellationToken ct = default);
}
