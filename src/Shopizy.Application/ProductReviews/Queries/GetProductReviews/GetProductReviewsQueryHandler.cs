using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler(
    IProductReviewRepository productReviewRepository,
    IProductRepository productRepository
) : IQueryHandler<GetProductReviewsQuery, ErrorOr<List<ProductReview>>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<List<ProductReview>>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken
    )
    {
        var productId = ProductId.Create(request.ProductId);
        var productExists = await _productRepository.IsProductExistAsync(productId);
        if (!productExists)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        var reviews = await _productReviewRepository.GetReviewsByProductIdAsync(productId);
        return reviews.ToList();
    }
}
