using Shouldly;

namespace Shopizy.Application.ProductReviews.Commands.CreateProductReview.UnitTests;

/// <summary>
/// Unit tests for <see cref="CreateProductReviewCommandValidator"/>.
/// </summary>
public class CreateProductReviewCommandValidatorTests
{
    /// <summary>
    /// Tests that the validator passes validation when all properties are valid.
    /// </summary>
    [Fact]
    public void Constructor_WithValidCommand_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 3m,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator fails when UserId is empty (Guid.Empty).
    /// </summary>
    [Fact]
    public void Constructor_WithEmptyUserId_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.Empty,
            ProductId: Guid.NewGuid(),
            Rating: 3m,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "UserId");
    }

    /// <summary>
    /// Tests that the validator fails when ProductId is empty (Guid.Empty).
    /// </summary>
    [Fact]
    public void Constructor_WithEmptyProductId_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.Empty,
            Rating: 3m,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
    }

    /// <summary>
    /// Tests that the validator fails when Rating is below the minimum allowed value.
    /// </summary>
    /// <param name="rating">The rating value to test.</param>
    [Theory]
    [InlineData(0)]
    [InlineData(0.99)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithRatingBelowMinimum_ShouldFailValidation(decimal rating)
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: rating,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "Rating");
    }

    /// <summary>
    /// Tests that the validator fails when Rating is above the maximum allowed value.
    /// </summary>
    /// <param name="rating">The rating value to test.</param>
    [Theory]
    [InlineData(5.01)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(100)]
    public void Constructor_WithRatingAboveMaximum_ShouldFailValidation(decimal rating)
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: rating,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "Rating");
    }

    /// <summary>
    /// Tests that the validator passes when Rating is at the minimum boundary (1).
    /// </summary>
    [Fact]
    public void Constructor_WithRatingAtMinimumBoundary_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 1m,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator passes when Rating is at the maximum boundary (5).
    /// </summary>
    [Fact]
    public void Constructor_WithRatingAtMaximumBoundary_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 5m,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator passes when Rating is within the valid range.
    /// </summary>
    /// <param name="rating">The rating value to test.</param>
    [Theory]
    [InlineData(1.5)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(4.5)]
    public void Constructor_WithValidRating_ShouldPassValidation(decimal rating)
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: rating,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator fails when Comment exceeds maximum length of 1000 characters.
    /// </summary>
    [Fact]
    public void Constructor_WithCommentExceedingMaximumLength_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 3m,
            Comment: new string('a', 1001)
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "Comment");
    }

    /// <summary>
    /// Tests that the validator passes when Comment is exactly at maximum length of 1000 characters.
    /// </summary>
    [Fact]
    public void Constructor_WithCommentAtMaximumLength_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 3m,
            Comment: new string('a', 1000)
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator passes when Comment is null or empty.
    /// MaximumLength validator allows null and empty strings by default.
    /// </summary>
    /// <param name="comment">The comment value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithNullOrEmptyComment_ShouldPassValidation(string? comment)
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: 3m,
            Comment: comment!
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.Count.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the validator reports multiple validation errors when multiple properties are invalid.
    /// </summary>
    [Fact]
    public void Constructor_WithMultipleInvalidProperties_ShouldFailValidationWithMultipleErrors()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.Empty,
            ProductId: Guid.Empty,
            Rating: 0m,
            Comment: new string('a', 1001)
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(4);
        result.Errors.ShouldContain(e => e.PropertyName == "UserId");
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
        result.Errors.ShouldContain(e => e.PropertyName == "Rating");
        result.Errors.ShouldContain(e => e.PropertyName == "Comment");
    }

    /// <summary>
    /// Tests that the validator fails when Rating is decimal.MinValue.
    /// </summary>
    [Fact]
    public void Constructor_WithRatingAsDecimalMinValue_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: decimal.MinValue,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "Rating");
    }

    /// <summary>
    /// Tests that the validator fails when Rating is decimal.MaxValue.
    /// </summary>
    [Fact]
    public void Constructor_WithRatingAsDecimalMaxValue_ShouldFailValidation()
    {
        // Arrange
        var validator = new CreateProductReviewCommandValidator();
        var command = new CreateProductReviewCommand(
            UserId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Rating: decimal.MaxValue,
            Comment: "Great product!"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldContain(e => e.PropertyName == "Rating");
    }
}
