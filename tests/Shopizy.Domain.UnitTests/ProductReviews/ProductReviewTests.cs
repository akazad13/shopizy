using Shouldly;
using Xunit;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.UnitTests.ProductReviews;

public class ProductReviewTests
{
    [Fact]
    public void Create_WithValidData_ReturnsProductReview()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(4.5m);
        var comment = "Great product!";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Id.ShouldNotBeNull();
        productReview.Id.Value.ShouldNotBe(Guid.Empty);
        productReview.UserId.ShouldBe(userId);
        productReview.ProductId.ShouldBe(productId);
        productReview.Rating.ShouldBe(rating);
        productReview.Comment.ShouldBe(comment);
    }
}
