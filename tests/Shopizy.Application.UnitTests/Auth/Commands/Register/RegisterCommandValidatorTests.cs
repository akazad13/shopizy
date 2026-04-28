using FluentValidation.TestHelper;
using Shopizy.Application.Auth.Commands.Register;

namespace Shopizy.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private const string ValidPassword = "Aa1!aaaaaaaa";
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Should_HaveError_WhenFirstNameIsEmpty()
    {
        var command = new RegisterCommand("", "Last", "test@test.com", ValidPassword);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_HaveError_WhenLastNameIsEmpty()
    {
        var command = new RegisterCommand("First", "", "test@test.com", ValidPassword);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        var command = new RegisterCommand("First", "Last", "not-an-email", ValidPassword);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_WhenPasswordIsEmpty()
    {
        var command = new RegisterCommand("First", "Last", "test@test.com", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("short")] // too short
    [InlineData("alllowercase!1a")] // no uppercase
    [InlineData("ALLUPPERCASE!1A")] // no lowercase
    [InlineData("NoDigitsHere!aa")] // no digit
    [InlineData("NoSpecialChar1aa")] // no special character
    public void Should_HaveError_WhenPasswordIsWeak(string weakPassword)
    {
        var command = new RegisterCommand("First", "Last", "test@test.com", weakPassword);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new RegisterCommand("First", "Last", "test@test.com", ValidPassword);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new RegisterCommand(
            new string('a', 51),
            "Last",
            "test@test.com",
            ValidPassword
        );
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_HaveError_WhenEmailExceedsMaxLength()
    {
        var command = new RegisterCommand(
            "First",
            "Last",
            new string('a', 246) + "@test.com",
            ValidPassword
        );
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
