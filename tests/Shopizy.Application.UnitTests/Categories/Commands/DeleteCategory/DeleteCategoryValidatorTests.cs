using FluentValidation.TestHelper;
using Shopizy.Application.Categories.Commands.DeleteCategory;

namespace Shopizy.Application.UnitTests.Categories.Commands.DeleteCategory;

public class DeleteCategoryValidatorTests
{
    private readonly DeleteCategoryValidator _validator;

    public DeleteCategoryValidatorTests()
    {
        _validator = new DeleteCategoryValidator();
    }

    [Fact]
    public async Task Should_NotHaveError_When_CommandIsValid()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_UserIdIsEmpty()
    {
        // Arrange - No validation rules exist, so even empty GUID should pass
        var command = new DeleteCategoryCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_CategoryIdIsEmpty()
    {
        // Arrange - No validation rules exist, so even empty GUID should pass
        var command = new DeleteCategoryCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_BothIdsAreEmpty()
    {
        // Arrange - No validation rules exist
        var command = new DeleteCategoryCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
