using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.ProductReviews;

public sealed class ProductReview : AggregateRoot<ProductReviewId, Guid>
{
    public CustomerId CustomerId { get; set; }
    public ProductId ProductId { get; set; }
    public Rating Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public static ProductReview Create(
        CustomerId customerId,
        ProductId productId,
        Rating rating,
        string comment
    )
    {
        return new ProductReview(
            ProductReviewId.CreateUnique(),
            customerId,
            productId,
            rating,
            comment,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private ProductReview(
        ProductReviewId productReviewId,
        CustomerId customerId,
        ProductId productId,
        Rating rating,
        string comment,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(productReviewId)
    {
        CustomerId = customerId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ProductReview() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
