using TravelAgency.PaymentService.Entity;
using TravelAgency.PaymentService.Managers.Contracts;
using TravelAgency.PaymentService.Repository.Contracts;

namespace TravelAgency.PaymentService.Managers;

public class PaymentManager : IPaymentManager
{
    private readonly IPaymentRepository _repository;
    private readonly ILogger<PaymentManager> _logger;

    public PaymentManager(IPaymentRepository repository, ILogger<PaymentManager> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> ProcessPaymentAsync(Guid bookingId, decimal amount, CancellationToken ct = default)
    {
        // Проверка на дубликат (идемпотентность)
        var existing = await _repository.GetByBookingIdAsync(bookingId, ct);
        if (existing.Any())
        {
            _logger.LogWarning("Платёж для {BookingId} уже существует, пропускаю.", bookingId);
            return false;
        }

        // Имитация обработки
        await Task.Delay(2000, ct);

        var payment = new PaymentRecord
        {
            BookingId = bookingId,
            Amount = amount,
            ProcessedAt = DateTime.UtcNow,
            Status = "Success"
        };

        await _repository.AddAsync(payment, ct);
        _logger.LogInformation("Оплата сохранена: {BookingId}, сумма: {Amount}", bookingId, amount);
        return true;
    }

    public Task<IReadOnlyList<PaymentRecord>> GetPaymentsByBookingAsync(Guid bookingId, CancellationToken ct = default)
        => _repository.GetByBookingIdAsync(bookingId, ct);
}
