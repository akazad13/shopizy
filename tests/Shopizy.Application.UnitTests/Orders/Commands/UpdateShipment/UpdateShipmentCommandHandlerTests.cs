using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.UpdateShipment;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.UpdateShipment;

public class UpdateShipmentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly UpdateShipmentCommandHandler _sut;

    public UpdateShipmentCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new UpdateShipmentCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnOrderNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var command = new UpdateShipmentCommand(
            Guid.NewGuid(),
            "DHL",
            "TRACK789",
            DateTime.UtcNow.AddDays(3),
            (int)ShipmentStatus.InTransit
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
    public async Task Should_ReturnShipmentNotFound_WhenOrderHasNoShipment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = new UpdateShipmentCommand(
            order.Id.Value,
            "DHL",
            "TRACK789",
            DateTime.UtcNow.AddDays(3),
            (int)ShipmentStatus.InTransit
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Order.ShipmentNotFound", result.FirstError.Code);
    }

    [Fact]
    public async Task Should_UpdateShipment_WhenOrderAndShipmentExist()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.AddShipment("FedEx", "TRACK123", DateTime.UtcNow.AddDays(5));

        var command = new UpdateShipmentCommand(
            order.Id.Value,
            "DHL",
            "TRACK789",
            DateTime.UtcNow.AddDays(3),
            (int)ShipmentStatus.Delivered
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Success, result.Value);
        Assert.NotNull(order.Shipment);
        Assert.Equal("DHL", order.Shipment.Carrier);
        Assert.Equal("TRACK789", order.Shipment.TrackingNumber);
        Assert.Equal(ShipmentStatus.Delivered, order.Shipment.Status);
    }
}
