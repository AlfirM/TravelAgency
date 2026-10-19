using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }
        public DbSet<BookingEntity> Bookings => Set<BookingEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingEntity>(e =>
            {
                e.ToTable("bookings");
                e.HasKey(x => x.Id);
                e.Property(x => x.Price).HasPrecision(18, 2);
            });

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
