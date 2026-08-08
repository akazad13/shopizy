using ErrorOr;
using FluentValidation.TestHelper;
using Moq;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Admin.Queries.GetTopProducts;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Admin.Queries.GetTopProducts;

public class GetTopProductsQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetTopProductsQueryHandler _handler;

    public GetTopProductsQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new GetTopProductsQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTopProductsList()
    {
        // Arrange
        var query = new GetTopProductsQuery(5);
        var expectedProducts = new List<TopProductDto> { new("Top Product", 100, 5000.00m) };

        _mockOrderRepository
            .Setup(r => r.GetTopProductsByRevenueAsync(5))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Name.ShouldBe("Top Product");
        result.Value[0].Revenue.ShouldBe(5000.00m);
    }
}

public class GetTopProductsQueryValidatorTests
{
    private readonly GetTopProductsQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Should_HaveError_When_CountIsOutOfRange(int count)
    {
        var query = new GetTopProductsQuery(count);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Should_NotHaveError_When_CountIsValid(int count)
    {
        var query = new GetTopProductsQuery(count);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
