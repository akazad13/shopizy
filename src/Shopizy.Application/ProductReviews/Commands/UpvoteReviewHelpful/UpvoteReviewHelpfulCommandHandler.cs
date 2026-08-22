using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.UpvoteReviewHelpful;

public class UpvoteReviewHelpfulCommandHandler(IProductReviewRepository productReviewRepository)
    : ICommandHandler<UpvoteReviewHelpfulCommand, ErrorOr<ProductReview>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;

    public async Task<ErrorOr<ProductReview>> Handle(
        UpvoteReviewHelpfulCommand request,
        CancellationToken cancellationToken
    )
    {
        var review = await _productReviewRepository.GetProductReviewByIdAsync(
            ProductReviewId.Create(request.ReviewId)
        );

        if (review is null)
        {
            return (Error)CustomErrors.ProductReview.ReviewNotFound;
        }

        review.UpvoteHelpful();
        _productReviewRepository.Update(review);

        return review;
    }
}
