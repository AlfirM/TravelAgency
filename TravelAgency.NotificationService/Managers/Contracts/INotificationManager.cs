namespace TravelAgency.NotificationService.Managers.Contracts;

public interface INotificationManager
{
    Task SendBookingConfirmationAsync(Guid bookingId, string customerName, string destination, CancellationToken ct = default);
}
