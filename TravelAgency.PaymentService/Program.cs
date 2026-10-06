using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelAgency.PaymentService;
using TravelAgency.PaymentService.Consumers;
using TravelAgency.PaymentService.Managers;
using TravelAgency.PaymentService.Managers.Contracts;
using TravelAgency.PaymentService.Repository;
using TravelAgency.PaymentService.Repository.Contracts;
using TravelAgency.PaymentService.Managers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentManager, PaymentManager>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingPlacedConsumer>();
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
    scope.ServiceProvider.GetRequiredService<PaymentDbContext>().Database.EnsureCreated();
}

await host.RunAsync();