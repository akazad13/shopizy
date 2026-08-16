using FluentValidation.TestHelper;
using Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;

namespace Shopizy.Application.UnitTests.ProductQuestions.Commands.AnswerQuestion;

public class AnswerQuestionCommandValidatorTests
{
    private readonly AnswerQuestionCommandValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenCommandIsValid()
    {
        var command = new AnswerQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), "Valid answer");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenQuestionIdIsEmpty()
    {
        var command = new AnswerQuestionCommand(Guid.Empty, Guid.NewGuid(), "Valid answer");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Fact]
    public void Should_HaveError_WhenAnsweredByUserIdIsEmpty()
    {
        var command = new AnswerQuestionCommand(Guid.NewGuid(), Guid.Empty, "Valid answer");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AnsweredByUserId);
    }

    [Fact]
    public void Should_HaveError_WhenAnswerIsEmpty()
    {
        var command = new AnswerQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Answer);
    }
}
