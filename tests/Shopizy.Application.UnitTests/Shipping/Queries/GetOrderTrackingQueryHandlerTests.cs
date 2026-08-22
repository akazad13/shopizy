using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Shipping.Queries.GetOrderTracking;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Shipping.Queries;

public class GetOrderTrackingQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo = new();
    private readonly Mock<IShippingCarrierService> _mockCarrierService = new();
    private readonly GetOrderTrackingQueryHandler _sut;

    public GetOrderTrackingQueryHandlerTests()
    {
        _sut = new GetOrderTrackingQueryHandler(_mockOrderRepo.Object, _mockCarrierService.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnOrderNotFoundError()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderTrackingQuery(orderId);

        _mockOrderRepo
            .Setup(x => x.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Order.OrderNotFound);
    }

    [Fact]
    public async Task Handle_WhenShipmentNotFound_ShouldReturnShipmentNotFoundError()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var query = new GetOrderTrackingQuery(order.Id.Value);

        _mockOrderRepo.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Shipment.ShipmentNotFound);
    }

    [Fact]
    public async Task Handle_WhenShipmentExists_ShouldReturnTrackingInfo()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        order.AddShipment("FedEx", "123456789", DateTime.UtcNow.AddDays(2));

        var query = new GetOrderTrackingQuery(order.Id.Value);
        var expectedTracking = new ShippingTrackingInfoDto(
            "FedEx",
            "123456789",
            "InTransit",
            "Local Hub",
            DateTime.UtcNow.AddDays(2),
            []
        );

        _mockOrderRepo.Setup(x => x.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        _mockCarrierService
            .Setup(x => x.TrackShipmentAsync("FedEx", "123456789", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTracking);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Carrier.ShouldBe("FedEx");
        result.Value.TrackingNumber.ShouldBe("123456789");
    }
}
