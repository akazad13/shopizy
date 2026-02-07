using FluentValidation.TestHelper;
using Shopizy.Application.Categories.Commands.CreateCategory;

namespace Shopizy.Application.UnitTests.Categories.Commands.CreateCategory;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _validator = new CreateCategoryValidator();
    }

    [Fact]
    public async Task Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var command = new CreateCategoryCommand(Guid.NewGuid(), "", null);

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_HaveError_When_NameIsNull()
    {
        // Arrange
        var command = new CreateCategoryCommand(Guid.NewGuid(), null!, null);

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
        var command = new CreateCategoryCommand(Guid.NewGuid(), longName, null);

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_NotHaveError_When_NameIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand(Guid.NewGuid(), "Electronics", null);

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
        var command = new CreateCategoryCommand(Guid.NewGuid(), maxLengthName, null);

        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new CreateCategoryCommand(
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
