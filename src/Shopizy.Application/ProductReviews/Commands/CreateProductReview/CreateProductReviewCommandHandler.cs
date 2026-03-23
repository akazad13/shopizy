using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewCommandHandler(
    IProductReviewRepository productReviewRepository,
    IProductRepository productRepository
) : ICommandHandler<CreateProductReviewCommand, ErrorOr<ProductReview>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductReview>> Handle(
        CreateProductReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId.Create(request.ProductId));
        if (product is null)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        var rating = Rating.CreateNew(request.Rating);
        var review = ProductReview.Create(
            UserId.Create(request.UserId),
            ProductId.Create(request.ProductId),
            rating,
            request.Comment
        );

        await _productReviewRepository.AddAsync(review);

        product.AddReviewRating(rating);
        _productRepository.Update(product);

        return review;
    }
}
