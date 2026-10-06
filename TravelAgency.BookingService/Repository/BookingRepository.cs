using Microsoft.EntityFrameworkCore;
using TravelAgency.BookingService.Repository.Contracts;
using TravelAgency.Contracts.Entity;
using TravelAgency.Contracts.Repository;

namespace TravelAgency.BookingService.Repository;

public class BookingRepository : EntityRepository<BookingEntity>, IBookingRepository
{
    public BookingRepository(BookingDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BookingEntity>> GetByCustomerAsync(string customerName, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(b => b.CustomerName == customerName)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<BookingEntity>> GetPendingAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(b => b.Status == "Pending")
            .ToListAsync(ct);
}
