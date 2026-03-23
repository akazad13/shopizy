using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;

public class AnswerQuestionCommandHandler(IProductQuestionRepository productQuestionRepository)
    : ICommandHandler<AnswerQuestionCommand, ErrorOr<ProductQuestion>>
{
    private readonly IProductQuestionRepository _productQuestionRepository = productQuestionRepository;

    public async Task<ErrorOr<ProductQuestion>> Handle(
        AnswerQuestionCommand request,
        CancellationToken cancellationToken
    )
    {
        var question = await _productQuestionRepository.GetByIdAsync(
            ProductQuestionId.Create(request.QuestionId)
        );

        if (question is null)
        {
            return CustomErrors.ProductQuestion.QuestionNotFound;
        }

        var result = question.AddAnswer(UserId.Create(request.AnsweredByUserId), request.Answer);
        if (result.IsError)
        {
            return result.Errors;
        }

        _productQuestionRepository.Update(question);

        return question;
    }
}
