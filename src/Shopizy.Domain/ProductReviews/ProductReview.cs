using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.ProductReviews;

public sealed class ProductReview : AggregateRoot<ProductReviewId, Guid>
{
    public UserId UserId { get; set; }
    public ProductId ProductId { get; set; }
    public Rating Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public static ProductReview Create(
        UserId userId,
        ProductId productId,
        Rating rating,
        string comment
    )
    {
        return new ProductReview(
            ProductReviewId.CreateUnique(),
            userId,
            productId,
            rating,
            comment
        );
    }

    private ProductReview(
        ProductReviewId productReviewId,
        UserId userId,
        ProductId productId,
        Rating rating,
        string comment
    ) : base(productReviewId)
    {
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
        CreatedOn = DateTime.UtcNow;
    }

    private ProductReview() { }
}
