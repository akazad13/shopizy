using ErrorOr;
using Shopizy.Domain.ProductQuestions;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;

public record GetProductQuestionsQuery(Guid ProductId) : IQuery<ErrorOr<IReadOnlyList<ProductQuestion>>>;
