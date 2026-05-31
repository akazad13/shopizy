using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.Events;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

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

    /// <summary>
    /// Tests that Delete method adds a ProductReviewDeletedDomainEvent to the DomainEvents collection
    /// with the correct ProductId and Rating values from the product review.
    /// </summary>
    [Fact]
    public void Delete_WhenCalled_AddsProductReviewDeletedDomainEvent()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(4.5m);
        var comment = "Great product!";
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Act
        productReview.Delete();

        // Assert
        productReview.DomainEvents.ShouldNotBeEmpty();
        productReview.DomainEvents.ShouldContain(e => e is ProductReviewDeletedDomainEvent);
    }

    /// <summary>
    /// Tests that Delete method adds a domain event with the correct ProductId and Rating
    /// that match the product review's properties.
    /// </summary>
    [Fact]
    public void Delete_WhenCalled_DomainEventContainsCorrectProductIdAndRating()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(3.0m);
        var comment = "Average product";
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Act
        productReview.Delete();

        // Assert
        var domainEvent = productReview
            .DomainEvents.OfType<ProductReviewDeletedDomainEvent>()
            .FirstOrDefault();
        domainEvent.ShouldNotBeNull();
        domainEvent.ProductId.ShouldBe(productId);
        domainEvent.Rating.ShouldBe(rating);
    }

    /// <summary>
    /// Tests that calling Delete multiple times adds multiple ProductReviewDeletedDomainEvent instances
    /// to the DomainEvents collection.
    /// </summary>
    [Fact]
    public void Delete_WhenCalledMultipleTimes_AddsMultipleDomainEvents()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(5.0m);
        var comment = "Excellent!";
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Act
        productReview.Delete();
        productReview.Delete();

        // Assert
        var deletedEvents = productReview
            .DomainEvents.OfType<ProductReviewDeletedDomainEvent>()
            .ToList();
        deletedEvents.Count.ShouldBe(2);
        deletedEvents[0].ProductId.ShouldBe(productId);
        deletedEvents[0].Rating.ShouldBe(rating);
        deletedEvents[1].ProductId.ShouldBe(productId);
        deletedEvents[1].Rating.ShouldBe(rating);
    }

    /// <summary>
    /// Tests that Create method returns a valid ProductReview with all properties correctly set
    /// when provided with valid input parameters.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ReturnsProductReviewWithCorrectProperties()
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

    /// <summary>
    /// Tests that Create method adds a ProductReviewCreatedDomainEvent to the domain events collection
    /// when creating a new product review.
    /// </summary>
    [Fact]
    public void Create_WithValidData_AddsDomainEvent()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(3.5m);
        var comment = "Good quality";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.DomainEvents.ShouldNotBeEmpty();
        productReview.DomainEvents.ShouldContain(e => e is ProductReviewCreatedDomainEvent);
        var domainEvent = productReview
            .DomainEvents.OfType<ProductReviewCreatedDomainEvent>()
            .FirstOrDefault();
        domainEvent.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that Create method generates a unique ProductReviewId for each new review instance.
    /// </summary>
    [Fact]
    public void Create_CalledMultipleTimes_GeneratesUniqueIds()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(5.0m);
        var comment = "Excellent!";

        // Act
        var productReview1 = ProductReview.Create(userId, productId, rating, comment);
        var productReview2 = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview1.Id.ShouldNotBe(productReview2.Id);
        productReview1.Id.Value.ShouldNotBe(productReview2.Id.Value);
    }

    /// <summary>
    /// Tests that Create method correctly handles an empty string comment by creating a review with an empty comment.
    /// </summary>
    [Fact]
    public void Create_WithEmptyComment_ReturnsProductReviewWithEmptyComment()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(3.0m);
        var comment = string.Empty;

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Comment.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Tests that Create method correctly handles a whitespace-only comment by creating a review with that comment.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceComment_ReturnsProductReviewWithWhitespaceComment()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(2.5m);
        var comment = "   ";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Comment.ShouldBe("   ");
    }

    /// <summary>
    /// Tests that Create method correctly handles a very long comment string.
    /// </summary>
    [Fact]
    public void Create_WithVeryLongComment_ReturnsProductReviewWithLongComment()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(4.0m);
        var comment = new string('A', 10000);

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Comment.ShouldBe(comment);
        productReview.Comment.Length.ShouldBe(10000);
    }

    /// <summary>
    /// Tests that Create method correctly handles comments containing special characters.
    /// </summary>
    [Fact]
    public void Create_WithSpecialCharactersInComment_ReturnsProductReviewWithSpecialCharacters()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(4.5m);
        var comment = "Great! @#$%^&*() <html> \n\r\t 日本語 émojis: 😀🎉";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Comment.ShouldBe(comment);
    }

    /// <summary>
    /// Tests that Create method works correctly with various rating values including edge cases.
    /// </summary>
    /// <param name="ratingValue"></param>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2.5)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_WithVariousRatingValues_ReturnsProductReviewWithCorrectRating(
        decimal ratingValue
    )
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(ratingValue);
        var comment = "Review comment";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Rating.ShouldBe(rating);
        productReview.Rating.Value.ShouldBe(ratingValue);
    }

    /// <summary>
    /// Tests that Create method works correctly with negative rating values.
    /// </summary>
    [Fact]
    public void Create_WithNegativeRating_ReturnsProductReviewWithNegativeRating()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(-1.0m);
        var comment = "Negative rating test";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Rating.Value.ShouldBe(-1.0m);
    }

    /// <summary>
    /// Tests that Create method works correctly with extreme decimal rating values.
    /// </summary>
    [Fact]
    public void Create_WithExtremeRatingValue_ReturnsProductReviewWithExtremeRating()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(decimal.MaxValue);
        var comment = "Extreme rating test";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Rating.Value.ShouldBe(decimal.MaxValue);
    }

    /// <summary>
    /// Tests that Create method properly initializes all navigation properties to their default values.
    /// </summary>
    [Fact]
    public void Create_WithValidData_InitializesNavigationPropertiesToNull()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(4.0m);
        var comment = "Test comment";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.User.ShouldBeNull();
    }

    /// <summary>
    /// Tests that Create method with comment containing only newlines creates a review correctly.
    /// </summary>
    [Fact]
    public void Create_WithCommentContainingOnlyNewlines_ReturnsProductReviewWithNewlineComment()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var productId = ProductId.CreateUnique();
        var rating = Rating.CreateNew(3.0m);
        var comment = "\n\n\n";

        // Act
        var productReview = ProductReview.Create(userId, productId, rating, comment);

        // Assert
        productReview.ShouldNotBeNull();
        productReview.Comment.ShouldBe("\n\n\n");
    }
}
