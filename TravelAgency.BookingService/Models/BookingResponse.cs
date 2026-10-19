// TravelAgency.BookingService/Models/BookingResponse.cs
namespace TravelAgency.BookingService.Models
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}