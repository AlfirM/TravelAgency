// TravelAgency.BookingService/Program.cs
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BookingService;
using TravelAgency.BookingService.Managers;
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Messaging;
using TravelAgency.BookingService.Repository;
using TravelAgency.BookingService.Repository.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingManager, BookingManager>();
builder.Services.AddScoped<IBookingEventPublisher, BookingEventPublisher>();

builder.Services.AddMassTransit(x =>
{
    // Включаем Transactional Outbox на базе EF Core
    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        o.UsePostgres();          // диалект SQL для outbox-таблиц
        o.UseBusOutbox();         // публикация идёт через outbox, а не напрямую
        o.QueryDelay = TimeSpan.FromSeconds(1);      // как часто проверять outbox
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(10);
    });

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();