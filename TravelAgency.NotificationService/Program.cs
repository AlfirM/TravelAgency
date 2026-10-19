using MassTransit;
using MassTransit.SqlTransport;
using Microsoft.EntityFrameworkCore;
using TravelAgency.NotificationService;
using TravelAgency.NotificationService.Consumers;
using TravelAgency.NotificationService.Managers;
using TravelAgency.NotificationService.Managers.Contracts;
using TravelAgency.NotificationService.Repository;
using TravelAgency.NotificationService.Repository.Contracts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationManager, NotificationManager>();

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

using (var scope = host.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.EnsureCreated();
}

await host.RunAsync();