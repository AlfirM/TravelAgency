// TravelAgency.BookingService/Program.cs
using MassTransit;
using TravelAgency.Contracts.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрируем MassTransit и настраиваем RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Адрес RabbitMQ.
        // В Docker-окружении имя хоста — это имя сервиса "rabbitmq"
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // MassTransit сам создаст очереди для потребителей
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// 2. Определяем API-эндпоинт
app.MapPost("/api/bookings", async (BookingRequest request, IPublishEndpoint publishEndpoint) =>
{
    // Имитация сохранения бронирования в БД (опущено)
    var bookingId = Guid.NewGuid();

    // 3. Публикуем событие в RabbitMQ
    await publishEndpoint.Publish(new BookingPlaced
    {
        BookingId = bookingId,
        CustomerName = request.CustomerName,
        Destination = request.Destination,
        Price = request.Price
    });

    return Results.Ok(new { Message = "Бронирование принято, обрабатывается.", BookingId = bookingId });
});

app.Run();

// Простая модель запроса
public record BookingRequest(string CustomerName, string Destination, decimal Price);