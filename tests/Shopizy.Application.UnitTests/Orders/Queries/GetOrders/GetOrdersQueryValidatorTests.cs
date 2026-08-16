using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Queries.GetOrders;

namespace Shopizy.Application.UnitTests.Orders.Queries.GetOrders;

public class GetOrdersQueryValidatorTests
{
    private readonly GetOrdersQueryValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenPageNumberAndSizeAreValid()
    {
        // Arrange
        var query = new GetOrdersQuery(Guid.NewGuid(), null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenPageNumberIsZeroOrNegative()
    {
        // Arrange
        var query = new GetOrdersQuery(Guid.NewGuid(), null, null, null, 0, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsZeroOrNegative()
    {
        // Arrange
        var query = new GetOrdersQuery(Guid.NewGuid(), null, null, null, 1, 0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
