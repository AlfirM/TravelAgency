// TravelAgency.BookingService/Messaging/BookingEventPublisher.cs
using MassTransit;
using TravelAgency.Contracts;
using TravelAgency.Contracts.Data;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Messaging
{
    public class BookingEventPublisher : IBookingEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BookingEventPublisher> _logger;

        public BookingEventPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<BookingEventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishBookingPlacedAsync(BookingEntity booking, CancellationToken ct = default)
        {
            var message = new BookingPlaced
            {
                BookingId = booking.Id,
                CustomerName = booking.CustomerName,
                Destination = booking.Destination,
                Price = booking.Price,
                BookingDate = booking.BookingDate
            };

            await _publishEndpoint.Publish(message, ct);
            _logger.LogInformation("Опубликовано событие BookingPlaced для {BookingId}", booking.Id);
        }

        public async Task PublishBookingStatusChangedAsync(Guid bookingId, string status, CancellationToken ct = default)
        {
            // Пока контракта BookingStatusChanged нет — логируем.
            // Когда появится — добавите record в Contracts и опубликуете здесь.
            await Task.CompletedTask;
            _logger.LogInformation(
                "Статус бронирования {BookingId} изменён на {Status} (событие не публикуется — контракт отсутствует)",
                bookingId, status);
        }
    }
}