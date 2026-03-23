using ErrorOr;
using Shopizy.Domain.ProductReviews;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.ProductReviews.Queries.GetProductReviews;

public record GetProductReviewsQuery(Guid ProductId) : IQuery<ErrorOr<List<ProductReview>>>;
