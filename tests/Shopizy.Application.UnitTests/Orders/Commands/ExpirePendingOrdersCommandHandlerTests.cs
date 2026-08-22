using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.ExpirePendingOrders;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Orders.Commands;

public class ExpirePendingOrdersCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ExpirePendingOrdersCommandHandler>> _mockLogger;
    private readonly ExpirePendingOrdersCommandHandler _handler;

    public ExpirePendingOrdersCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<ExpirePendingOrdersCommandHandler>>();

        _handler = new ExpirePendingOrdersCommandHandler(
            _mockOrderRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_WhenNoExpiredOrders_ShouldReturnZero()
    {
        // Arrange
        var threshold = DateTime.UtcNow.AddMinutes(-15);
        var command = new ExpirePendingOrdersCommand(threshold);

        _mockOrderRepository
            .Setup(r =>
                r.GetExpiredPendingOrdersAsync(threshold, 50, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(0);
        _mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExpiredOrdersExist_ShouldCancelOrdersAndSave()
    {
        // Arrange
        var threshold = DateTime.UtcNow.AddMinutes(-15);
        var command = new ExpirePendingOrdersCommand(threshold, 10);

        var order1 = OrderFactory.CreateOrder();
        var order2 = OrderFactory.CreateOrder();

        _mockOrderRepository
            .Setup(r =>
                r.GetExpiredPendingOrdersAsync(threshold, 10, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<Order> { order1, order2 });

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(2);

        order1.OrderStatus.ShouldBe(OrderStatus.Cancelled);
        order2.OrderStatus.ShouldBe(OrderStatus.Cancelled);

        _mockOrderRepository.Verify(r => r.Update(order1), Times.Once);
        _mockOrderRepository.Verify(r => r.Update(order2), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
