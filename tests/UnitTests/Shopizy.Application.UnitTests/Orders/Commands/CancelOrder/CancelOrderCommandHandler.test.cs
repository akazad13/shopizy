using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly CancelOrderCommandHandler _sut;

    public CancelOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new CancelOrderCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnOrderIsNotFound_WhenOrderIdIsNotProvided()
    {
        // Arrange
        var command = CancelOrderCommandUtils.CreateCommand(Guid.Empty);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Order.OrderNotFound, result.Errors[0]);

        _mockOrderRepository.Verify(
            x => x.GetOrderByIdAsync(OrderId.Create(command.OrderId)),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_CancelTheOrder_WhenOrderExistsAndCancellationReasonIsValid()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = CancelOrderCommandUtils.CreateCommand(order.Id.Value);

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);
        _mockOrderRepository.Setup(x => x.Update(order));
        _mockOrderRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockOrderRepository.Verify(
            x => x.GetOrderByIdAsync(OrderId.Create(command.OrderId)),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.Update(order), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
