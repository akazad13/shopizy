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
        Order order = OrderFactory.CreateOrder();
        CancelOrderCommand command = CancelOrderCommandUtils.CreateCommand(order.Id);

        _ = _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(() => order);

        _ = _mockOrderRepository.Setup(x => x.Update(It.IsAny<Order>()));
        _ = _mockOrderRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        ErrorOr<Success> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockOrderRepository.Verify(x => x.GetOrderByIdAsync(order.Id), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType(typeof(Success));
    }
}
