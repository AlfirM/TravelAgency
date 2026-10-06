
namespace TravelAgency.Contracts.Entity;

public class BookingEntity : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Pending";
}
