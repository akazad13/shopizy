using ErrorOr;
using Shopizy.Domain.ProductQuestions;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;

public record AnswerQuestionCommand(Guid QuestionId, Guid AnsweredByUserId, string Answer) : ICommand<ErrorOr<ProductQuestion>>;
