using ErrorOr;
using Shopizy.Domain.ProductReviews;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview;

public record CreateProductReviewCommand(
    Guid UserId,
    Guid ProductId,
    decimal Rating,
    string Comment
) : ICommand<ErrorOr<ProductReview>>;
