using FluentValidation;

namespace Shopizy.Application.ProductQuestions.Commands.AskQuestion;

public class AskQuestionCommandValidator : AbstractValidator<AskQuestionCommand>
{
    public AskQuestionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Question).NotNull().NotEmpty().MaximumLength(1000);
    }
}
