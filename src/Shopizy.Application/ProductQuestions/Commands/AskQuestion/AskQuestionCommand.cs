using ErrorOr;
using Shopizy.Domain.ProductQuestions;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Commands.AskQuestion;

public record AskQuestionCommand(Guid UserId, Guid ProductId, string Question) : ICommand<ErrorOr<ProductQuestion>>;
