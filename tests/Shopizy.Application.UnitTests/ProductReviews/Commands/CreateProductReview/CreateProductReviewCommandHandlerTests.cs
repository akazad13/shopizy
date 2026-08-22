using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.ProductReviews;
using Shouldly;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview.UnitTests;

/// <summary>
/// Unit tests for <see cref="CreateProductReviewCommandHandler"/>.
/// </summary>
public class CreateProductReviewCommandHandlerTests
{
    private readonly Mock<IProductReviewRepository> _mockProductReviewRepo = new();
    private readonly Mock<IOrderRepository> _mockOrderRepo = new();

    /// <summary>
    /// Tests that Handle creates and returns a product review successfully with valid input.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidInput_ShouldCreateAndReturnProductReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = 4.5m;
        var comment = "Great product!";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        _mockProductReviewRepo
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        _mockOrderRepo
            .Setup(x =>
                x.HasUserPurchasedProductAsync(
                    It.IsAny<Shopizy.Domain.Users.ValueObjects.UserId>(),
                    It.IsAny<Shopizy.Domain.Products.ValueObjects.ProductId>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var handler = new CreateProductReviewCommandHandler(
            _mockProductReviewRepo.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<ProductReview>();
        result.Value.IsVerifiedPurchase.ShouldBeTrue();
        _mockProductReviewRepo.Verify(x => x.AddAsync(It.IsAny<ProductReview>()), Times.Once);
    }

    /// <summary>
    /// Tests that Handle creates a review with correct properties matching the command including Headline and Images.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateReviewWithCorrectProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = 3.5m;
        var comment = "Average product";
        var headline = "Not bad";
        var images = new List<string> { "https://example.com/photo1.jpg" };
        var command = new CreateProductReviewCommand(
            userId,
            productId,
            rating,
            comment,
            headline,
            images
        );

        _mockProductReviewRepo
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            _mockProductReviewRepo.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.Value.UserId.Value.ShouldBe(userId);
        result.Value.ProductId.Value.ShouldBe(productId);
        result.Value.Rating.Value.ShouldBe(rating);
        result.Value.Comment.ShouldBe(comment);
        result.Value.Headline.ShouldBe(headline);
        result.Value.ImageUrls.Count.ShouldBe(1);
        result.Value.ImageUrls[0].ShouldBe("https://example.com/photo1.jpg");
    }

    /// <summary>
    /// Tests that Handle creates a review with various rating values including edge cases.
    /// </summary>
    /// <param name="rating">The rating value to test.</param>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0.5)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(9999.99)]
    public async Task Handle_WithVariousRatingValues_ShouldCreateReview(decimal rating)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var comment = "Test comment";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Rating.Value.ShouldBe(rating);
    }

    /// <summary>
    /// Tests that Handle creates a review with various comment strings including edge cases.
    /// </summary>
    /// <param name="comment">The comment string to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Normal comment")]
    [InlineData("A")]
    [InlineData(
        "This is a very long comment that contains a lot of text to test edge cases with lengthy user input"
    )]
    [InlineData("Comment with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?")]
    [InlineData("\n\r\t")]
    public async Task Handle_WithVariousComments_ShouldCreateReview(string comment)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = 4.0m;
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Comment.ShouldBe(comment);
    }

    /// <summary>
    /// Tests that Handle creates a review even when UserId and ProductId are empty GUIDs.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyGuids_ShouldCreateReview()
    {
        // Arrange
        var userId = Guid.Empty;
        var productId = Guid.Empty;
        var rating = 3.0m;
        var comment = "Test comment";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.UserId.Value.ShouldBe(userId);
        result.Value.ProductId.Value.ShouldBe(productId);
    }

    /// <summary>
    /// Tests that Handle calls the repository AddAsync method exactly once with the created review.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallRepositoryAddAsyncOnce()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = 4.0m;
        var comment = "Test comment";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        ProductReview? capturedReview = null;
        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Callback<ProductReview>(review => capturedReview = review)
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        mockRepository.Verify(x => x.AddAsync(It.IsAny<ProductReview>()), Times.Once);
        capturedReview.ShouldNotBeNull();
        capturedReview.ShouldBe(result.Value);
    }

    /// <summary>
    /// Tests that Handle creates a review with decimal minimum value rating.
    /// </summary>
    [Fact]
    public async Task Handle_WithDecimalMinValue_ShouldCreateReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = decimal.MinValue;
        var comment = "Test comment";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Rating.Value.ShouldBe(decimal.MinValue);
    }

    /// <summary>
    /// Tests that Handle creates a review with decimal maximum value rating.
    /// </summary>
    [Fact]
    public async Task Handle_WithDecimalMaxValue_ShouldCreateReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = decimal.MaxValue;
        var comment = "Test comment";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Rating.Value.ShouldBe(decimal.MaxValue);
    }

    /// <summary>
    /// Tests that Handle returns the same ProductReview instance that was created.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnSameInstancePassedToRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var rating = 5.0m;
        var comment = "Excellent!";
        var command = new CreateProductReviewCommand(userId, productId, rating, comment);

        ProductReview? repositoryReview = null;
        var mockRepository = new Mock<IProductReviewRepository>();
        mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ProductReview>()))
            .Callback<ProductReview>(review => repositoryReview = review)
            .Returns(Task.CompletedTask);

        var handler = new CreateProductReviewCommandHandler(
            mockRepository.Object,
            _mockOrderRepo.Object
        );

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        repositoryReview.ShouldNotBeNull();
        result.Value.ShouldBeSameAs(repositoryReview);
    }
}
