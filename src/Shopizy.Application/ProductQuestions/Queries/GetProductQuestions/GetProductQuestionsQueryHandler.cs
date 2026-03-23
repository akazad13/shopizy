using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;

public class GetProductQuestionsQueryHandler(IProductQuestionRepository productQuestionRepository)
    : IQueryHandler<GetProductQuestionsQuery, ErrorOr<IReadOnlyList<ProductQuestion>>>
{
    private readonly IProductQuestionRepository _productQuestionRepository = productQuestionRepository;

    public async Task<ErrorOr<IReadOnlyList<ProductQuestion>>> Handle(
        GetProductQuestionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var questions = await _productQuestionRepository.GetByProductIdAsync(
            ProductId.Create(request.ProductId)
        );

        return ErrorOrFactory.From(questions);
    }
}
