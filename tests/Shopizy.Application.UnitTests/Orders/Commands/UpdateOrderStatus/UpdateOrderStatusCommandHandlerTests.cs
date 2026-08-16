using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.UpdateOrderStatus;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly UpdateOrderStatusCommandHandler _sut;

    public UpdateOrderStatusCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new UpdateOrderStatusCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ThrowArgumentNullException_WhenCommandIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.Handle(null!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Should_ReturnOrderNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            OrderStatus.Delivered
        );

        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Order.OrderNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_UpdateOrderStatus_WhenOrderExists()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = new UpdateOrderStatusCommand(
            order.UserId.Value,
            order.Id.Value,
            OrderStatus.Processing
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Success, result.Value);
        Assert.Equal(OrderStatus.Processing, order.OrderStatus);
    }
}
