// TravelAgency.Tests/Common/TestData.cs
using TravelAgency.Contracts;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.Tests.Common
{
    internal static class TestData
    {
        public static BookingEntity NewBooking(
            Guid? id = null,
            string customerName = "Иван Иванов",
            string destination = "Париж",
            decimal price = 100_000m,
            string status = "Pending")
        {
            return new BookingEntity
            {
                Id = id ?? Guid.NewGuid(),
                CustomerName = customerName,
                Destination = destination,
                Price = price,
                BookingDate = DateTime.UtcNow,
                Status = status
            };
        }
    }
}