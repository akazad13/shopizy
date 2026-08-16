using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Commands.CreateShipment;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateShipment;

public class CreateShipmentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly CreateShipmentCommandHandler _sut;

    public CreateShipmentCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _sut = new CreateShipmentCommandHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnOrderNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var command = new CreateShipmentCommand(
            Guid.NewGuid(),
            "UPS",
            "TRACK123",
            DateTime.UtcNow.AddDays(2)
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
    public async Task Should_CreateShipment_WhenOrderExists()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var command = new CreateShipmentCommand(
            order.Id.Value,
            "UPS",
            "TRACK123",
            DateTime.UtcNow.AddDays(2)
        );

        _mockOrderRepository.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal("UPS", result.Value.Carrier);
        Assert.Equal("TRACK123", result.Value.TrackingNumber);
    }

    [Fact]
    public async Task Should_ReturnError_WhenOrderAlreadyHasShipment()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.AddShipment("UPS", "TRACK123", DateTime.UtcNow.AddDays(2));

        var command = new CreateShipmentCommand(
            order.Id.Value,
            "DHL",
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
