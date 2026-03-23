using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductQuestions.Commands.AskQuestion;

public class AskQuestionCommandHandler(
    IProductQuestionRepository productQuestionRepository,
    IProductRepository productRepository
) : ICommandHandler<AskQuestionCommand, ErrorOr<ProductQuestion>>
{
    private readonly IProductQuestionRepository _productQuestionRepository = productQuestionRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductQuestion>> Handle(
        AskQuestionCommand request,
        CancellationToken cancellationToken
    )
    {
        var productId = ProductId.Create(request.ProductId);
        var product = await _productRepository.GetProductByIdAsync(productId);

        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        var question = ProductQuestion.Create(
            productId,
            UserId.Create(request.UserId),
            request.Question
        );

        await _productQuestionRepository.AddAsync(question);

        return question;
    }
}
