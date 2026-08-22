using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewCommandHandler(
    IProductReviewRepository productReviewRepository,
    IOrderRepository orderRepository
) : ICommandHandler<CreateProductReviewCommand, ErrorOr<ProductReview>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<ProductReview>> Handle(
        CreateProductReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(request.ProductId);
        var rating = Rating.CreateNew(request.Rating);

        var isVerifiedPurchase = await _orderRepository.HasUserPurchasedProductAsync(
            userId,
            productId,
            cancellationToken
        );

        var review = ProductReview.Create(
            userId,
            productId,
            rating,
            request.Comment,
            isVerifiedPurchase,
            request.Headline,
            request.ImageUrls
        );

        await _productReviewRepository.AddAsync(review);

        return review;
    }
}
