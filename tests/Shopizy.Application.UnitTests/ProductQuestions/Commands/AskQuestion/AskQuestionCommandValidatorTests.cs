using FluentValidation.TestHelper;
using Shopizy.Application.ProductQuestions.Commands.AskQuestion;

namespace Shopizy.Application.UnitTests.ProductQuestions.Commands.AskQuestion;

public class AskQuestionCommandValidatorTests
{
    private readonly AskQuestionCommandValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenCommandIsValid()
    {
        var command = new AskQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), "Valid question?");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenUserIdIsEmpty()
    {
        var command = new AskQuestionCommand(Guid.Empty, Guid.NewGuid(), "Valid question?");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_HaveError_WhenProductIdIsEmpty()
    {
        var command = new AskQuestionCommand(Guid.NewGuid(), Guid.Empty, "Valid question?");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void Should_HaveError_WhenQuestionIsEmpty()
    {
        var command = new AskQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Question);
    }
}
