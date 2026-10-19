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
            await _publishEndpoint.Publish(new BookingPlaced
            {
                BookingId = booking.Id,
                CustomerName = booking.CustomerName,
                Destination = booking.Destination,
                Price = booking.Price,
                BookingDate = booking.BookingDate
            }, ct);

            _logger.LogInformation(
                "BookingPlaced для {BookingId} поставлено в outbox", booking.Id);
        }

        public async Task PublishBookingStatusChangedAsync(Guid bookingId, string status, CancellationToken ct = default)
        {
            // Когда появится контракт BookingStatusChanged — опубликуете здесь.
            await Task.CompletedTask;
            _logger.LogInformation(
                "Статус {BookingId} изменён на {Status} (контракт события пока не определён)",
                bookingId, status);
        }
    }
}