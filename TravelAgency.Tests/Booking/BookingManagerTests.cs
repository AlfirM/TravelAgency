// TravelAgency.Tests/Booking/BookingManagerTests.cs
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TravelAgency.BookingService.Managers;
using TravelAgency.BookingService.Repository.Contracts;
using TravelAgency.Contracts;
using TravelAgency.Contracts.Entity;
using TravelAgency.Tests.Common;
using Xunit;

namespace TravelAgency.Tests.Booking
{
    public class BookingManagerTests
    {
        private readonly Mock<IBookingRepository> _repositoryMock;
        private readonly BookingManager _sut;   // System Under Test

        public BookingManagerTests()
        {
            _repositoryMock = new Mock<IBookingRepository>(MockBehavior.Strict);
            _sut = new BookingManager(_repositoryMock.Object, NullLogger<BookingManager>.Instance);
        }

        // ---------- CreateBookingAsync ----------

        [Fact]
        public async Task CreateBookingAsync_Should_SaveBooking_WithPendingStatus()
        {
            // Arrange
            BookingEntity? captured = null;

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
                .Callback<BookingEntity, CancellationToken>((b, _) => captured = b)
                .ReturnsAsync((BookingEntity b, CancellationToken _) => b);

            // Act
            var result = await _sut.CreateBookingAsync("Иван", "Париж", 100_000m);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Status.Should().Be("Pending");
            result.CustomerName.Should().Be("Иван");
            result.Destination.Should().Be("Париж");
            result.Price.Should().Be(100_000m);

            captured.Should().NotBeNull();
            captured!.Id.Should().Be(result.Id);

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_Should_ReturnBooking_WithGeneratedId()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingEntity b, CancellationToken _) => b);

            // Act
            var result = await _sut.CreateBookingAsync("A", "B", 1m);

            // Assert
            result.Id.Should().NotBe(Guid.Empty);
            result.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        // ---------- GetBookingAsync ----------

        [Fact]
        public async Task GetBookingAsync_WhenExists_Should_ReturnBooking()
        {
            // Arrange
            var booking = TestData.NewBooking();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _sut.GetBookingAsync(booking.Id);

            // Assert
            result.Should().BeSameAs(booking);
        }

        [Fact]
        public async Task GetBookingAsync_WhenNotFound_Should_ReturnNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingEntity?)null);

            // Act
            var result = await _sut.GetBookingAsync(id);

            // Assert
            result.Should().BeNull();
        }

        // ---------- GetAllBookingsAsync ----------

        [Fact]
        public async Task GetAllBookingsAsync_Should_ReturnAllFromRepository()
        {
            // Arrange
            var bookings = new List<BookingEntity>
            {
                TestData.NewBooking(),
                TestData.NewBooking(),
                TestData.NewBooking()
            };
            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _sut.GetAllBookingsAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(bookings);
        }

        // ---------- UpdateStatusAsync ----------

        [Fact]
        public async Task UpdateStatusAsync_WhenExists_Should_UpdateAndReturnTrue()
        {
            // Arrange
            var booking = TestData.NewBooking(status: "Pending");
            _repositoryMock
                .Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);
            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateStatusAsync(booking.Id, "Paid");

            // Assert
            result.Should().BeTrue();
            booking.Status.Should().Be("Paid");

            _repositoryMock.Verify(
                r => r.UpdateAsync(It.Is<BookingEntity>(b => b.Id == booking.Id && b.Status == "Paid"),
                                   It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenNotFound_Should_ReturnFalse_AndNotUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingEntity?)null);

            // Act
            var result = await _sut.UpdateStatusAsync(id, "Paid");

            // Assert
            result.Should().BeFalse();
            _repositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}