using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductReviews.Commands.UpvoteReviewHelpful;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.ProductReviews.Commands.UpvoteReviewHelpful;

public class UpvoteReviewHelpfulCommandHandlerTests
{
    private readonly Mock<IProductReviewRepository> _mockRepo = new();
    private readonly UpvoteReviewHelpfulCommandHandler _handler;

    public UpvoteReviewHelpfulCommandHandlerTests()
    {
        _handler = new UpvoteReviewHelpfulCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenReviewNotFound_ShouldReturnError()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var command = new UpvoteReviewHelpfulCommand(reviewId);

        _mockRepo
            .Setup(x => x.GetProductReviewByIdAsync(It.IsAny<ProductReviewId>()))
            .ReturnsAsync((ProductReview?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.ProductReview.ReviewNotFound);
    }

    [Fact]
    public async Task Handle_WhenReviewExists_ShouldIncrementHelpfulVotesAndReturnReview()
    {
        // Arrange
        var review = ProductReview.Create(
            UserId.CreateUnique(),
            ProductId.CreateUnique(),
            Rating.CreateNew(5m),
            "Great item!"
        );
        var initialVotes = review.HelpfulVotesCount;
        var command = new UpvoteReviewHelpfulCommand(review.Id.Value);

        _mockRepo.Setup(x => x.GetProductReviewByIdAsync(review.Id)).ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.HelpfulVotesCount.ShouldBe(initialVotes + 1);
        _mockRepo.Verify(x => x.Update(review), Times.Once);
    }
}
