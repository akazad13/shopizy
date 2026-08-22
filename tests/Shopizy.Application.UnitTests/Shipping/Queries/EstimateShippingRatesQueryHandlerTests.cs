using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Shipping.Queries.EstimateShippingRates;
using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Shipping.Queries;

public class EstimateShippingRatesQueryHandlerTests
{
    private readonly Mock<IShippingCarrierService> _mockCarrierService = new();
    private readonly EstimateShippingRatesQueryHandler _sut;

    public EstimateShippingRatesQueryHandlerTests()
    {
        _sut = new EstimateShippingRatesQueryHandler(_mockCarrierService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEstimatedRatesFromService()
    {
        // Arrange
        var query = new EstimateShippingRatesQuery(
            "123 Main St",
            "Austin",
            "TX",
            "US",
            "78701",
            2m,
            50m
        );
        var expectedRates = new List<ShippingRateEstimateDto>
        {
            new("USPS", "USPS_GROUND", "Standard Ground", 5.99m, "USD", 3, 5),
        };

        _mockCarrierService
            .Setup(x =>
                x.EstimateShippingRatesAsync(
                    It.IsAny<Address>(),
                    query.TotalWeightKg,
                    query.Subtotal,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedRates);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Carrier.ShouldBe("USPS");
    }
}
