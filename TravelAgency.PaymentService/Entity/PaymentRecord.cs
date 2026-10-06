using TravelAgency.Contracts.Entity;

namespace TravelAgency.PaymentService.Entity;

public class PaymentRecord : BaseEntity
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string Status { get; set; } = "Success";
}
