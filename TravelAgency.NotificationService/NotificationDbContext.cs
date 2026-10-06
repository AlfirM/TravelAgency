
using Microsoft.EntityFrameworkCore;
using TravelAgency.NotificationService.Entity;

namespace TravelAgency.NotificationService;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationLog> Notifications => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);
        });
    }
}