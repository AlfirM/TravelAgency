// TravelAgency.BookingService/Messaging/IBookingEventPublisher.cs
using TravelAgency.Contracts;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Messaging
{
    public interface IBookingEventPublisher
    {
        Task PublishBookingPlacedAsync(BookingEntity booking, CancellationToken ct = default);
        Task PublishBookingStatusChangedAsync(Guid bookingId, string status, CancellationToken ct = default);
    }
}