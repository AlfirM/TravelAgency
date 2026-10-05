// TravelAgency.PaymentService/Program.cs
using MassTransit;
using TravelAgency.Contracts.Data;

var builder = Host.CreateApplicationBuilder(args);

// 1. Регистрируем MassTransit и потребителя
builder.Services.AddMassTransit(x =>
{
    // Добавляем потребителя — MassTransit автоматически зарегистрирует его в DI
    x.AddConsumer<BookingPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // MassTransit сам создаст очередь для потребителя.
        // Например, очередь "payment-service" по правилам форматирования по умолчанию
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
await host.RunAsync();

// 2. Логика потребителя
public class BookingPlacedConsumer : IConsumer<BookingPlaced>
{
    private readonly ILogger<BookingPlacedConsumer> _logger;

    public BookingPlacedConsumer(ILogger<BookingPlacedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingPlaced> context)
    {
        var message = context.Message;

        _logger.LogInformation("Получено событие бронирования: {BookingId}. Обрабатываю оплату...", message.BookingId);

        // Имитация обработки платежа
        await Task.Delay(2000);

        _logger.LogInformation("Оплата обработана: {BookingId}, сумма: {Price}", message.BookingId, message.Price);

        // В реальном приложении здесь вы бы опубликовали событие PaymentCompleted
        // Для демонстрации мы просто логируем
    }
}