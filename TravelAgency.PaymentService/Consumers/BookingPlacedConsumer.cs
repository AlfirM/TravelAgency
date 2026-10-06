using MassTransit;
using TravelAgency.Contracts.Data;
using TravelAgency.PaymentService.Managers.Contracts;

namespace TravelAgency.PaymentService.Consumers;

public class BookingPlacedConsumer : IConsumer<BookingPlaced>
{
    private readonly IPaymentManager _paymentManager;
    private readonly ILogger<BookingPlacedConsumer> _logger;

    public BookingPlacedConsumer(IPaymentManager paymentManager, ILogger<BookingPlacedConsumer> logger)
    {
        _paymentManager = paymentManager;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingPlaced> context)
    {
        var message = context.Message;
        _logger.LogInformation("Обработка оплаты для {BookingId}", message.BookingId);
        await _paymentManager.ProcessPaymentAsync(message.BookingId, message.Price, context.CancellationToken);
    }
}
