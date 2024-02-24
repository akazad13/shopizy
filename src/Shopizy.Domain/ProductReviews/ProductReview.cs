using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.ProductReviews;

public sealed class ProductReview : AggregateRoot<ProductReviewId, Guid>
{
    public Rating Rating { get; set; }
    public string Comment { get; set; }
    public CustomerId CustomerId { get; set; }
    public ProductId ProductId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public static ProductReview Create(
        Rating rating,
        string comment,
        CustomerId customerId,
        ProductId productId
    )
    {
        return new ProductReview(
            ProductReviewId.CreateUnique(),
            rating,
            comment,
            customerId,
            productId,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private ProductReview(
        ProductReviewId productReviewId,
        Rating rating,
        string comment,
        CustomerId customerId,
        ProductId productId,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(productReviewId)
    {
        Rating = rating;
        Comment = comment;
        CustomerId = customerId;
        ProductId = productId;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ProductReview() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
