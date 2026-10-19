// TravelAgency.BookingService/Models/BookingRequest.cs
using System.ComponentModel.DataAnnotations;

namespace TravelAgency.BookingService.Models
{
    public class BookingRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Destination { get; set; } = string.Empty;

        [Range(0.01, 1_000_000)]
        public decimal Price { get; set; }
    }
}