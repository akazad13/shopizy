using FluentValidation.TestHelper;
using Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;

namespace Shopizy.Application.UnitTests.ProductQuestions.Queries.GetProductQuestions;

public class GetProductQuestionsQueryValidatorTests
{
    private readonly GetProductQuestionsQueryValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenQueryIsValid()
    {
        var query = new GetProductQuestionsQuery(Guid.NewGuid(), 1, 10);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenPageNumberIsZero()
    {
        var query = new GetProductQuestionsQuery(Guid.NewGuid(), 0, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsZero()
    {
        var query = new GetProductQuestionsQuery(Guid.NewGuid(), 1, 0);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
