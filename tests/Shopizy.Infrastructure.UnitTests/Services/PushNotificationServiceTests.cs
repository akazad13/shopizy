using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Infrastructure.Services.Notifications;
using Shouldly;

namespace Shopizy.Infrastructure.UnitTests.Services;

public class PushNotificationServiceTests
{
    private readonly Mock<ILogger<PushNotificationService>> _mockLogger;
    private readonly PushNotificationService _service;

    public PushNotificationServiceTests()
    {
        _mockLogger = new Mock<ILogger<PushNotificationService>>();
        _service = new PushNotificationService(_mockLogger.Object);
    }

    [Fact]
    public async Task SendPushNotificationAsync_WhenUserIdEmptyOrTitleEmpty_ShouldReturnFalse()
    {
        var result1 = await _service.SendPushNotificationAsync(Guid.Empty, "Title", "Body");
        result1.ShouldBeFalse();

        var result2 = await _service.SendPushNotificationAsync(Guid.NewGuid(), "", "Body");
        result2.ShouldBeFalse();
    }

    [Fact]
    public async Task SendPushNotificationAsync_WhenValid_ShouldReturnTrue()
    {
        var result = await _service.SendPushNotificationAsync(
            Guid.NewGuid(),
            "Order Shipped",
            "Your order is on the way"
        );
        result.ShouldBeTrue();
    }
}
