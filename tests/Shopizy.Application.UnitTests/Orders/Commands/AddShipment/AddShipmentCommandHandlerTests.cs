using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.AddShipment;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.AddShipment;

public class AddShipmentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly AddShipmentCommandHandler _sut;

    public AddShipmentCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new AddShipmentCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnOrderNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var command = new AddShipmentCommand(
            Guid.NewGuid(),
            "FedEx",
            "TRACK123",
            DateTime.UtcNow.AddDays(3)
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
    public async Task Should_AddShipment_WhenOrderExistsAndNoShipmentExists()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = new AddShipmentCommand(
            order.Id.Value,
            "DHL",
            "TRACK456",
            DateTime.UtcNow.AddDays(5)
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("DHL", result.Value.Carrier);
        Assert.Equal("TRACK456", result.Value.TrackingNumber);
    }

    [Fact]
    public async Task Should_ReturnError_WhenOrderAlreadyHasShipment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.AddShipment("DHL", "TRACK456", DateTime.UtcNow.AddDays(5));

        var command = new AddShipmentCommand(
            order.Id.Value,
            "FedEx",
            "TRACK789",
            DateTime.UtcNow.AddDays(3)
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Order.ShipmentExists", result.FirstError.Code);
    }
}
