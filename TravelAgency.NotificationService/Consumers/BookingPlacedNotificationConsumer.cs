using MassTransit;
using TravelAgency.Contracts.Data;
using TravelAgency.NotificationService.Managers.Contracts;

namespace TravelAgency.NotificationService.Consumers;

public class BookingPlacedNotificationConsumer : IConsumer<BookingPlaced>
{
    private readonly INotificationManager _notificationManager;
    private readonly ILogger<BookingPlacedNotificationConsumer> _logger;

    public BookingPlacedNotificationConsumer(
        INotificationManager notificationManager,
        ILogger<BookingPlacedNotificationConsumer> logger)
    {
        _notificationManager = notificationManager;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingPlaced> context)
    {
        var m = context.Message;
        _logger.LogInformation("Отправка уведомления для {BookingId}", m.BookingId);
        await _notificationManager.SendBookingConfirmationAsync(
            m.BookingId, m.CustomerName, m.Destination, context.CancellationToken);
    }
}
