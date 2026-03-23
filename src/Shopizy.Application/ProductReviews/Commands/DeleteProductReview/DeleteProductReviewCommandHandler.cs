using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.DeleteProductReview;

public class DeleteProductReviewCommandHandler(
    IProductReviewRepository productReviewRepository,
    IProductRepository productRepository
) : ICommandHandler<DeleteProductReviewCommand, ErrorOr<Deleted>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<Deleted>> Handle(
        DeleteProductReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var review = await _productReviewRepository.GetProductReviewByIdAsync(
            ProductReviewId.Create(request.ReviewId)
        );
        if (review is null)
        {
            return CustomErrors.ProductReview.ReviewNotFound;
        }

        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(request.ProductId));
        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        _productReviewRepository.Remove(review);
        product.RemoveReviewRating(review.Rating);
        _productRepository.Update(product);

        return Result.Deleted;
    }
}
