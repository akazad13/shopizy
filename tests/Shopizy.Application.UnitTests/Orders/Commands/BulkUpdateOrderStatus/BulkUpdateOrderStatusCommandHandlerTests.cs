using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.BulkUpdateOrderStatus;

public class BulkUpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly BulkUpdateOrderStatusCommandHandler _sut;

    public BulkUpdateOrderStatusCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new BulkUpdateOrderStatusCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_UpdateOrderStatusForMatchingOrders_WhenOrdersExist()
    {
        // Arrange
        var order1 = OrderFactory.CreateOrder();
        var order2 = OrderFactory.CreateOrder();
        var command = new BulkUpdateOrderStatusCommand(
            new List<Guid> { order1.Id.Value, order2.Id.Value },
            (int)OrderStatus.Delivered
        );

        _mockOrderRepository
            .Setup(x => x.GetOrdersByIdsAsync(It.IsAny<IList<OrderId>>()))
            .ReturnsAsync([order1, order2]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Success, result.Value);
        Assert.Equal(OrderStatus.Delivered, order1.OrderStatus);
        Assert.Equal(OrderStatus.Delivered, order2.OrderStatus);
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenNoOrdersMatch()
    {
        // Arrange
        var command = new BulkUpdateOrderStatusCommand(
            new List<Guid> { Guid.NewGuid() },
            (int)OrderStatus.Processing
        );

        _mockOrderRepository
            .Setup(x => x.GetOrdersByIdsAsync(It.IsAny<IList<OrderId>>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Success, result.Value);
    }
}
