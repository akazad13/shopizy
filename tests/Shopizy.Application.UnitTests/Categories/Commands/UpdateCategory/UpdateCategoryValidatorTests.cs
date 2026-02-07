using FluentValidation.TestHelper;
using Shopizy.Application.Categories.Commands.UpdateCategory;

namespace Shopizy.Application.UnitTests.Categories.Commands.UpdateCategory;

public class UpdateCategoryValidatorTests
{
    private readonly UpdateCategoryValidator _validator;

    public UpdateCategoryValidatorTests()
    {
        _validator = new UpdateCategoryValidator();
    }

    [Fact]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_HaveError_When_NameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('A', 101);
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            longName,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Updated Electronics",
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_NotHaveError_When_NameIsAtMaxLength()
    {
        // Arrange
        var maxLengthName = new string('A', 100);
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            maxLengthName,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Electronics",
            Guid.NewGuid()
        );

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
