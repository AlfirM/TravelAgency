// TravelAgency.BookingService/Models/BookingMapper.cs
using TravelAgency.Contracts;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Models
{
    public static class BookingMapper
    {
        public static BookingResponse ToResponse(this BookingEntity entity) => new()
        {
            Id = entity.Id,
            CustomerName = entity.CustomerName,
            Destination = entity.Destination,
            Price = entity.Price,
            BookingDate = entity.BookingDate,
            Status = entity.Status
        };
    }
}