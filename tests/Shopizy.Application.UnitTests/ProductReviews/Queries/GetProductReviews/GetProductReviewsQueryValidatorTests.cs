using FluentValidation.TestHelper;
using Shopizy.Application.ProductReviews.Queries.GetProductReviews;

namespace Shopizy.Application.UnitTests.ProductReviews.Queries.GetProductReviews;

public class GetProductReviewsQueryValidatorTests
{
    private readonly GetProductReviewsQueryValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenQueryIsValid()
    {
        var query = new GetProductReviewsQuery(Guid.NewGuid(), 1, 10);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenPageNumberIsZero()
    {
        var query = new GetProductReviewsQuery(Guid.NewGuid(), 0, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsZero()
    {
        var query = new GetProductReviewsQuery(Guid.NewGuid(), 1, 0);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
