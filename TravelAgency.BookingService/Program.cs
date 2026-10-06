using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BookingService;
using TravelAgency.BookingService.Managers;
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Repository;
using TravelAgency.BookingService.Repository.Contracts;
using TravelAgency.BookingService.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingManager, BookingManager>();

builder.Services.AddMassTransit(x =>
{
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BookingDbContext>().Database.EnsureCreated();
}

app.MapPost("/api/bookings", async (BookingRequest request, IBookingManager manager) =>
{
    var booking = await manager.CreateBookingAsync(request.CustomerName, request.Destination, request.Price);
    return Results.Ok(new { Message = "Бронирование принято.", BookingId = booking.Id });
});

app.MapGet("/api/bookings/{id}", async (Guid id, IBookingManager manager) =>
{
    var booking = await manager.GetBookingAsync(id);
    return booking is null ? Results.NotFound() : Results.Ok(booking);
});

app.MapGet("/api/bookings", async (IBookingManager manager) =>
    Results.Ok(await manager.GetAllBookingsAsync()));

app.Run();

public record BookingRequest(string CustomerName, string Destination, decimal Price);