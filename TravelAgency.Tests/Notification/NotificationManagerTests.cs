// TravelAgency.Tests/Notification/NotificationManagerTests.cs
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TravelAgency.NotificationService;
using TravelAgency.NotificationService.Entity;
using TravelAgency.NotificationService.Managers;
using TravelAgency.NotificationService.Repository.Contracts;
using Xunit;

namespace TravelAgency.Tests.Notification
{
    public class NotificationManagerTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly NotificationManager _sut;

        public NotificationManagerTests()
        {
            _repositoryMock = new Mock<INotificationRepository>(MockBehavior.Strict);
            _sut = new NotificationManager(_repositoryMock.Object, NullLogger<NotificationManager>.Instance);
        }

        [Fact]
        public async Task SendBookingConfirmationAsync_WhenNoExistingNotification_Should_Create()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            NotificationLog? captured = null;

            _repositoryMock
                .Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<NotificationLog>());
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
                .Callback<NotificationLog, CancellationToken>((n, _) => captured = n)
                .ReturnsAsync((NotificationLog n, CancellationToken _) => n);

            // Act
            await _sut.SendBookingConfirmationAsync(bookingId, "Иван", "Париж");

            // Assert
            captured.Should().NotBeNull();
            captured!.BookingId.Should().Be(bookingId);
            captured.CustomerName.Should().Be("Иван");
            captured.Message.Should().Contain("Париж");
            captured.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendBookingConfirmationAsync_WhenAlreadySent_Should_NotSendAgain()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new NotificationLog { BookingId = bookingId } });

            // Act
            await _sut.SendBookingConfirmationAsync(bookingId, "Иван", "Париж");

            // Assert
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}