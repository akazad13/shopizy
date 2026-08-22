using ErrorOr;
using Shopizy.Domain.ProductReviews;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview;

public record CreateProductReviewCommand(
    Guid UserId,
    Guid ProductId,
    decimal Rating,
    string Comment,
    string? Headline = null,
    IReadOnlyList<string>? ImageUrls = null
) : ICommand<ErrorOr<ProductReview>>;
