using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.ProductReviews;

/// <summary>
/// Represents a product review submitted by a user.
/// </summary>
public sealed class ProductReview : AggregateRoot<ProductReviewId, Guid>, IAuditable
{
    /// <summary>
    /// Gets or sets the user identifier who wrote the review.
    /// </summary>
    public UserId UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the user who wrote the review.
    /// </summary>
    public User User { get; set; }
    
    /// <summary>
    /// Gets or sets the product identifier being reviewed.
    /// </summary>
    public ProductId ProductId { get; set; }
    
    /// <summary>
    /// Gets or sets the rating given to the product.
    /// </summary>
    public Rating Rating { get; set; }
    
    /// <summary>
    /// Gets or sets the review comment.
    /// </summary>
    public string Comment { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the review was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the review was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Creates a new product review.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="rating">The rating.</param>
    /// <param name="comment">The review comment.</param>
    /// <returns>A new <see cref="ProductReview"/> instance.</returns>
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
    )
        : base(productReviewId)
    {
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
    }

    private ProductReview() { }
}
