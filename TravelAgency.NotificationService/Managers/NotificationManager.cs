using TravelAgency.NotificationService.Entity;
using TravelAgency.NotificationService.Managers.Contracts;
using TravelAgency.NotificationService.Repository.Contracts;

namespace TravelAgency.NotificationService.Managers;

public class NotificationManager : INotificationManager
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationManager> _logger;

    public NotificationManager(INotificationRepository repository, ILogger<NotificationManager> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(
        Guid bookingId, string customerName, string destination, CancellationToken ct = default)
    {
        // Проверка на дубликат
        var existing = await _repository.GetByBookingIdAsync(bookingId, ct);
        if (existing.Any())
        {
            _logger.LogWarning("Уведомление для {BookingId} уже отправлено.", bookingId);
            return;
        }

        // Имитация отправки
        await Task.Delay(1000, ct);

        var log = new NotificationLog
        {
            BookingId = bookingId,
            CustomerName = customerName,
            Message = $"Подтверждение бронирования в {destination}",
            SentAt = DateTime.UtcNow
        };

        await _repository.AddAsync(log, ct);
        _logger.LogInformation("Уведомление сохранено для {BookingId}", bookingId);
    }
}
