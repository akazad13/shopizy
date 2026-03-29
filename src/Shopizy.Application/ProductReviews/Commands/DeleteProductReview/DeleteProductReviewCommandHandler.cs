using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.DeleteProductReview;

public class DeleteProductReviewCommandHandler(
    IProductReviewRepository productReviewRepository
) : ICommandHandler<DeleteProductReviewCommand, ErrorOr<Deleted>>
{
    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;

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
            return (Error)CustomErrors.ProductReview.ReviewNotFound;
        }

        review.Delete();
        _productReviewRepository.Remove(review);

        return Result.Deleted;
    }
}
