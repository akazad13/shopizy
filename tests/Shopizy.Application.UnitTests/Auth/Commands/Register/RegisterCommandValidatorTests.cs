using FluentValidation.TestHelper;
using Shopizy.Application.Auth.Commands.Register;

namespace Shopizy.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public async Task Should_HaveError_When_FirstNameIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand("", "Doe", "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_HaveError_When_FirstNameIsNull()
    {
        // Arrange
        var command = new RegisterCommand(null!, "Doe", "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_HaveError_When_FirstNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('A', 51);
        var command = new RegisterCommand(longName, "Doe", "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_HaveError_When_LastNameIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand("John", "", "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_HaveError_When_LastNameIsNull()
    {
        // Arrange
        var command = new RegisterCommand("John", null!, "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_HaveError_When_LastNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('B', 51);
        var command = new RegisterCommand("John", longName, "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailIsNull()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", null!, "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailFormatIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "invalid-email", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', 45) + "@test.com"; // 54 chars total
        var command = new RegisterCommand("John", "Doe", longEmail, "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_PasswordIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@example.com", "");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Should_HaveError_When_PasswordIsNull()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@example.com", null!);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "test@example.com", "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("user+tag@example.com")]
    public async Task Should_NotHaveError_When_EmailFormatIsValid(string validEmail)
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", validEmail, "password123");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
