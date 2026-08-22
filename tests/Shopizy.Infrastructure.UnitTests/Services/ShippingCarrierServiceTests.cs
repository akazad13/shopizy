using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Infrastructure.Services.Shipping;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Services;

public class ShippingCarrierServiceTests
{
    private readonly Mock<ILogger<ShippingCarrierService>> _mockLogger = new();
    private readonly ShippingSettings _settings = new();
    private readonly ShippingCarrierService _sut;

    public ShippingCarrierServiceTests()
    {
        var options = Options.Create(_settings);
        _sut = new ShippingCarrierService(options, _mockLogger.Object);
    }

    [Fact]
    public async Task EstimateShippingRatesAsync_SubtotalAboveThreshold_ShouldOfferFreeStandardShipping()
    {
        // Arrange
        var address = Address.CreateNew("123 Main St", "Dallas", "TX", "US", "75001");

        // Act
        var rates = await _sut.EstimateShippingRatesAsync(
            address,
            totalWeightKg: 1.5m,
            subtotal: 120m
        );

        // Assert
        rates.ShouldNotBeEmpty();
        var usps = rates.First(r => r.Carrier == "USPS");
        usps.Rate.ShouldBe(0m);
        usps.ServiceName.ShouldContain("Free");
    }

    [Fact]
    public async Task EstimateShippingRatesAsync_InternationalAddress_ShouldApplyMultiplierAndNoFreeShipping()
    {
        // Arrange
        var address = Address.CreateNew("456 Queen St", "Toronto", "ON", "CA", "M5V2A8");

        // Act
        var rates = await _sut.EstimateShippingRatesAsync(
            address,
            totalWeightKg: 2m,
            subtotal: 200m
        );

        // Assert
        rates.ShouldNotBeEmpty();
        var usps = rates.First(r => r.Carrier == "USPS");
        usps.Rate.ShouldBeGreaterThan(0m);
        rates.ShouldContain(r => r.Carrier == "DHL");
    }

    [Fact]
    public async Task TrackShipmentAsync_ValidCarrierAndNumber_ShouldReturnTrackingCheckpoints()
    {
        // Act
        var tracking = await _sut.TrackShipmentAsync("FedEx", "123456789012");

        // Assert
        tracking.ShouldNotBeNull();
        tracking.Carrier.ShouldBe("FedEx");
        tracking.TrackingNumber.ShouldBe("123456789012");
        tracking.Checkpoints.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task TrackShipmentAsync_EmptyCarrier_ShouldReturnNull()
    {
        // Act
        var tracking = await _sut.TrackShipmentAsync("", "123456789012");

        // Assert
        tracking.ShouldBeNull();
    }
}
