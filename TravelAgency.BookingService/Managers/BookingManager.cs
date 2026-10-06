using MassTransit;
using TravelAgency.BookingService.Managers.Contracts;
using TravelAgency.BookingService.Repository.Contracts;
using TravelAgency.Contracts.Data;
using TravelAgency.Contracts.Entity;

namespace TravelAgency.BookingService.Managers;

public class BookingManager : IBookingManager
{
    private readonly IBookingRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<BookingManager> _logger;

    public BookingManager(
        IBookingRepository repository,
        IPublishEndpoint publishEndpoint,
        ILogger<BookingManager> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<BookingEntity> CreateBookingAsync(
        string customerName, string destination, decimal price, CancellationToken ct = default)
    {
        var booking = new BookingEntity
        {
            CustomerName = customerName,
            Destination = destination,
            Price = price,
            BookingDate = DateTime.UtcNow,
            Status = "Pending"
        };

        await _repository.AddAsync(booking, ct);
        _logger.LogInformation("Бронирование создано: {BookingId}", booking.Id);

        await _publishEndpoint.Publish(new BookingPlaced
        {
            BookingId = booking.Id,
            CustomerName = booking.CustomerName,
            Destination = booking.Destination,
            Price = booking.Price
        }, ct);

        return booking;
    }

    public Task<BookingEntity?> GetBookingAsync(Guid id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<BookingEntity>> GetAllBookingsAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);

    public async Task UpdateStatusAsync(Guid bookingId, string status, CancellationToken ct = default)
    {
        var booking = await _repository.GetByIdAsync(bookingId, ct);
        if (booking is null)
        {
            _logger.LogWarning("Бронирование {BookingId} не найдено", bookingId);
            return;
        }

        booking.Status = status;
        await _repository.UpdateAsync(booking, ct);
    }
}
