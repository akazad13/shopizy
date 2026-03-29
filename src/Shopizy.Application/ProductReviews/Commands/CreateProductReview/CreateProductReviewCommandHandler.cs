using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewCommandHandler(
    IProductReviewRepository productReviewRepository
) : ICommandHandler<CreateProductReviewCommand, ErrorOr<ProductReview>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;

    public async Task<ErrorOr<ProductReview>> Handle(
        CreateProductReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var rating = Rating.CreateNew(request.Rating);
        var review = ProductReview.Create(
            UserId.Create(request.UserId),
            ProductId.Create(request.ProductId),
            rating,
            request.Comment
        );

        await _productReviewRepository.AddAsync(review);

        return review;
    }
}
