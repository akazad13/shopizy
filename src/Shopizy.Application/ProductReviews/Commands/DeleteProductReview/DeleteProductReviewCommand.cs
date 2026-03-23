using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.DeleteProductReview;

public record DeleteProductReviewCommand(Guid ProductId, Guid ReviewId)
    : ICommand<ErrorOr<Deleted>>;
