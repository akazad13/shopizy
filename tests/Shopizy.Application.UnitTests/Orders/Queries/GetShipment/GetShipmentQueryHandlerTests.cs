using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Queries.GetShipment;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Queries.GetShipment;

public class GetShipmentQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetShipmentQueryHandler _sut;

    public GetShipmentQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new GetShipmentQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnOrderNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var query = new GetShipmentQuery(Guid.NewGuid(), Guid.NewGuid());

        _mockOrderRepository
            .Setup(x => x.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Order.OrderNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_ReturnForbidden_WhenOrderDoesNotBelongToUser()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = new GetShipmentQuery(Guid.NewGuid(), order.Id.Value); // Different UserId

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Forbidden, result.FirstError.Type);
        Assert.Equal("Order.Forbidden", result.FirstError.Code);
    }

    [Fact]
    public async Task Should_ReturnShipmentNotFound_WhenOrderHasNoShipment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = new GetShipmentQuery(order.UserId.Value, order.Id.Value);

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal("Order.ShipmentNotFound", result.FirstError.Code);
    }

    [Fact]
    public async Task Should_ReturnShipment_WhenOrderBelongsToUserAndHasShipment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.AddShipment("FedEx", "TRACK999", DateTime.UtcNow.AddDays(4));

        var query = new GetShipmentQuery(order.UserId.Value, order.Id.Value);

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("FedEx", result.Value.Carrier);
        Assert.Equal("TRACK999", result.Value.TrackingNumber);
    }
}
