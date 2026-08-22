using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Infrastructure.Realtime.Hubs;
using Shopizy.Infrastructure.Realtime.Services;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Realtime;

public class RealtimeNotifierTests
{
    private readonly Mock<IHubContext<OrderStatusHub>> _mockOrderHub = new();
    private readonly Mock<IHubContext<AdminDashboardHub>> _mockAdminHub = new();
    private readonly Mock<IHubClients> _mockOrderClients = new();
    private readonly Mock<IHubClients> _mockAdminClients = new();
    private readonly Mock<IClientProxy> _mockClientProxy = new();
    private readonly Mock<ILogger<RealtimeNotifier>> _mockLogger = new();
    private readonly RealtimeNotifier _sut;

    public RealtimeNotifierTests()
    {
        _mockOrderHub.Setup(h => h.Clients).Returns(_mockOrderClients.Object);
        _mockAdminHub.Setup(h => h.Clients).Returns(_mockAdminClients.Object);

        _mockOrderClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

        _mockAdminClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

        _sut = new RealtimeNotifier(_mockOrderHub.Object, _mockAdminHub.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SendOrderStatusUpdateAsync_ShouldSendToUserGroup()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var status = "Delivered";

        // Act
        await _sut.SendOrderStatusUpdateAsync(userId, orderId, status);

        // Assert
        _mockOrderClients.Verify(c => c.Group($"user-{userId}"), Times.Once);
        _mockClientProxy.Verify(
            c =>
                c.SendCoreAsync(
                    "ReceiveOrderStatusUpdate",
                    It.Is<object[]>(args => args.Length > 0),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task SendAdminMetricUpdateAsync_ShouldSendToAdminsGroup()
    {
        // Arrange
        var metricType = "OrderCreated";
        var data = new { OrderId = Guid.NewGuid(), Total = 99.99m };

        // Act
        await _sut.SendAdminMetricUpdateAsync(metricType, data);

        // Assert
        _mockAdminClients.Verify(c => c.Group(AdminDashboardHub.AdminGroup), Times.Once);
        _mockClientProxy.Verify(
            c =>
                c.SendCoreAsync(
                    "ReceiveMetricUpdate",
                    It.Is<object[]>(args => args.Length > 0),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
