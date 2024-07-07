using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.UnitTests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private readonly CancelOrderCommandHandler _handler;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public CancelOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new CancelOrderCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task CancelOrder_OrderExists_CancelOrderAndReturnSuccess()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = CancelOrderCommandUtils.CreateCommand(order.Id);

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(() => order);

        _mockOrderRepository.Setup(x => x.Update(It.IsAny<Order>()));
        _mockOrderRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockOrderRepository.Verify(x => x.GetOrderByIdAsync(order.Id), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(Success));
    }
}
