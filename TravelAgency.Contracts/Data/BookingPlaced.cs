namespace TravelAgency.Contracts.Data;

// record — неизменяемое сообщение с лаконичным синтаксисом
public record BookingPlaced
{
    public Guid BookingId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime BookingDate { get; init; } = DateTime.UtcNow;
}

// Опционально: событие о завершении оплаты
public record PaymentCompleted
{
    public Guid BookingId { get; init; }
    public DateTime PaymentDate { get; init; } = DateTime.UtcNow;
    public string Status { get; init; } = "Success";
}
