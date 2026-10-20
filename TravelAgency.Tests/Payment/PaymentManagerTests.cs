// TravelAgency.Tests/Payment/PaymentManagerTests.cs
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TravelAgency.PaymentService;
using TravelAgency.PaymentService.Entity;
using TravelAgency.PaymentService.Managers;
using TravelAgency.PaymentService.Repository.Contracts;
using Xunit;

namespace TravelAgency.Tests.Payment
{
    public class PaymentManagerTests
    {
        private readonly Mock<IPaymentRepository> _repositoryMock;
        private readonly PaymentManager _sut;

        public PaymentManagerTests()
        {
            _repositoryMock = new Mock<IPaymentRepository>(MockBehavior.Strict);
            _sut = new PaymentManager(_repositoryMock.Object, NullLogger<PaymentManager>.Instance);
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenNoExistingPayment_Should_CreatePayment()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            PaymentRecord? captured = null;

            _repositoryMock
                .Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<PaymentRecord>());
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<PaymentRecord>(), It.IsAny<CancellationToken>()))
                .Callback<PaymentRecord, CancellationToken>((p, _) => captured = p)
                .ReturnsAsync((PaymentRecord p, CancellationToken _) => p);

            // Act
            var result = await _sut.ProcessPaymentAsync(bookingId, 123_456m);

            // Assert
            result.Should().BeTrue();
            captured.Should().NotBeNull();
            captured!.BookingId.Should().Be(bookingId);
            captured.Amount.Should().Be(123_456m);
            captured.Status.Should().Be("Success");
            captured.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<PaymentRecord>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenPaymentAlreadyExists_Should_ReturnFalse_AndNotCreate()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var existing = new PaymentRecord { BookingId = bookingId, Amount = 100m };

            _repositoryMock
                .Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { existing });

            // Act
            var result = await _sut.ProcessPaymentAsync(bookingId, 100m);

            // Assert
            result.Should().BeFalse();
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<PaymentRecord>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GetPaymentsByBookingAsync_Should_DelegateToRepository()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var list = new List<PaymentRecord>
            {
                new() { BookingId = bookingId, Amount = 100m },
                new() { BookingId = bookingId, Amount = 200m }
            };
            _repositoryMock
                .Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            // Act
            var result = await _sut.GetPaymentsByBookingAsync(bookingId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(list);
        }
    }
}