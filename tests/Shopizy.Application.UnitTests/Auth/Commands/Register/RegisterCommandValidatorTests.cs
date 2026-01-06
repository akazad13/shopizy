using FluentValidation.TestHelper;
using Shopizy.Application.Auth.Commands.Register;
using Shouldly;

namespace Shopizy.Application.UnitTests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Should_HaveError_WhenFirstNameIsEmpty()
    {
        var command = new RegisterCommand("", "Last", "test@test.com", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_HaveError_WhenLastNameIsEmpty()
    {
        var command = new RegisterCommand("First", "", "test@test.com", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        var command = new RegisterCommand("First", "Last", "not-an-email", "password");
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

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new RegisterCommand("First", "Last", "test@test.com", "password");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new RegisterCommand(new string('a', 51), "Last", "test@test.com", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_HaveError_WhenEmailExceedsMaxLength()
    {
        var command = new RegisterCommand("First", "Last", new string('a', 42) + "@test.com", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
