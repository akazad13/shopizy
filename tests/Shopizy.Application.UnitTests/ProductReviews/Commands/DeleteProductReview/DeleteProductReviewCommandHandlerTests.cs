using System;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductReviews.Commands.DeleteProductReview;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shouldly;
using Xunit;


namespace Shopizy.Application.ProductReviews.Commands.DeleteProductReview.UnitTests;

/// <summary>
/// Unit tests for <see cref="DeleteProductReviewCommandHandler"/>.
/// </summary>
public class DeleteProductReviewCommandHandlerTests
{
    /// <summary>
    /// Tests that Handle returns ReviewNotFound error when the product review does not exist.
    /// Verifies that Delete() and Remove() are not called.
    /// </summary>
    [Fact]
    public async Task Handle_NonExistentReview_ReturnsReviewNotFoundError()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new DeleteProductReviewCommand(productId, reviewId);

        var mockRepository = new Mock<IProductReviewRepository>();

        mockRepository
            .Setup(x => x.GetProductReviewByIdAsync(It.Is<ProductReviewId>(id => id.Value == reviewId)))
            .ReturnsAsync((ProductReview?)null);

        var handler = new DeleteProductReviewCommandHandler(mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("ProductReview.ReviewNotFound");
        result.Errors[0].Description.ShouldBe("Product review is not found.");
        mockRepository.Verify(x => x.Remove(It.IsAny<ProductReview>()), Times.Never);
    }

    /// <summary>
    /// Tests that Handle works correctly with Guid.Empty as ReviewId.
    /// Ensures the repository is called with the correct ProductReviewId.
    /// </summary>
    [Fact]
    public async Task Handle_EmptyGuidReviewId_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var reviewId = Guid.Empty;
        var productId = Guid.NewGuid();
        var command = new DeleteProductReviewCommand(productId, reviewId);

        var mockRepository = new Mock<IProductReviewRepository>();

        mockRepository
            .Setup(x => x.GetProductReviewByIdAsync(It.Is<ProductReviewId>(id => id.Value == reviewId)))
            .ReturnsAsync((ProductReview?)null);

        var handler = new DeleteProductReviewCommandHandler(mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(
            x => x.GetProductReviewByIdAsync(It.Is<ProductReviewId>(id => id.Value == Guid.Empty)),
            Times.Once);
    }

    /// <summary>
    /// Tests that Handle calls GetProductReviewByIdAsync with the correct ProductReviewId
    /// constructed from the command's ReviewId.
    /// </summary>
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("12345678-1234-1234-1234-123456789012")]
    [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public async Task Handle_VariousGuids_CallsRepositoryWithCorrectProductReviewId(string guidString)
    {
        // Arrange
        var reviewId = Guid.Parse(guidString);
        var productId = Guid.NewGuid();
        var command = new DeleteProductReviewCommand(productId, reviewId);

        var mockRepository = new Mock<IProductReviewRepository>();

        mockRepository
            .Setup(x => x.GetProductReviewByIdAsync(It.IsAny<ProductReviewId>()))
            .ReturnsAsync((ProductReview?)null);

        var handler = new DeleteProductReviewCommandHandler(mockRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(
            x => x.GetProductReviewByIdAsync(It.Is<ProductReviewId>(id => id.Value == reviewId)),
            Times.Once);
    }
}