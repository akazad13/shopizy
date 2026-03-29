using FluentValidation;

namespace Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;

public class AnswerQuestionCommandValidator : AbstractValidator<AnswerQuestionCommand>
{
    public AnswerQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.AnsweredByUserId).NotEmpty();
        RuleFor(x => x.Answer).NotNull().NotEmpty().MaximumLength(1000);
    }
}
