using TravelAgency.Contracts.Entity;

namespace TravelAgency.NotificationService.Entity;

public class NotificationLog : BaseEntity
{
    public Guid BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
