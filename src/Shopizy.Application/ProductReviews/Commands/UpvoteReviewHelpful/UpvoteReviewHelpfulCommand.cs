using ErrorOr;
using Shopizy.Domain.ProductReviews;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.UpvoteReviewHelpful;

public record UpvoteReviewHelpfulCommand(Guid ReviewId) : ICommand<ErrorOr<ProductReview>>;
