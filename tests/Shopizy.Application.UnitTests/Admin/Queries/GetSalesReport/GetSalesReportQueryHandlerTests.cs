using ErrorOr;
using FluentValidation.TestHelper;
using Moq;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Admin.Queries.GetSalesReport;

public class GetSalesReportQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetSalesReportQueryHandler _handler;

    public GetSalesReportQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new GetSalesReportQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSalesReportDto()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var query = new GetSalesReportQuery(startDate, endDate);
        var topProducts = new List<TopProductDto> { new("Best Seller", 50, 2500.00m) };

        _mockOrderRepository
            .Setup(r => r.GetRevenueByPeriodAsync(startDate, endDate))
            .ReturnsAsync(5000.00m);
        _mockOrderRepository
            .Setup(r => r.GetOrdersCountByPeriodAsync(startDate, endDate))
            .ReturnsAsync(40);
        _mockOrderRepository
            .Setup(r => r.GetTopProductsByRevenueAsync(10))
            .ReturnsAsync(topProducts);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.StartDate.ShouldBe(startDate);
        result.Value.EndDate.ShouldBe(endDate);
        result.Value.TotalRevenue.ShouldBe(5000.00m);
        result.Value.TotalOrders.ShouldBe(40);
        result.Value.TopProducts.Count.ShouldBe(1);
    }
}

public class GetSalesReportQueryValidatorTests
{
    private readonly GetSalesReportQueryValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_StartDateIsEmpty()
    {
        var query = new GetSalesReportQuery(default, DateTime.UtcNow);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_HaveError_When_EndDateIsEmpty()
    {
        var query = new GetSalesReportQuery(DateTime.UtcNow, default);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_HaveError_When_EndDateIsBeforeStartDate()
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);
        var query = new GetSalesReportQuery(startDate, endDate);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_HaveError_When_DateRangeExceeds90Days()
    {
        var startDate = DateTime.UtcNow.AddDays(-91);
        var endDate = DateTime.UtcNow;
        var query = new GetSalesReportQuery(startDate, endDate);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_NotHaveError_When_DateRangeIsValid()
    {
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var query = new GetSalesReportQuery(startDate, endDate);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
