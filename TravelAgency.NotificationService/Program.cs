// TravelAgency.NotificationService/Program.cs
using MassTransit;
using TravelAgency.Contracts.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingPlacedNotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
await host.RunAsync();

public class BookingPlacedNotificationConsumer : IConsumer<BookingPlaced>
{
    private readonly ILogger<BookingPlacedNotificationConsumer> _logger;

    public BookingPlacedNotificationConsumer(ILogger<BookingPlacedNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingPlaced> context)
    {
        var message = context.Message;
        _logger.LogInformation("Получено событие бронирования: {BookingId}. Отправляю письмо-подтверждение клиенту {CustomerName}...", message.BookingId, message.CustomerName);

        // Имитация отправки письма
        await Task.Delay(1000);

        _logger.LogInformation("Письмо отправлено: {CustomerName}, направление: {Destination}", message.CustomerName, message.Destination);
    }
}