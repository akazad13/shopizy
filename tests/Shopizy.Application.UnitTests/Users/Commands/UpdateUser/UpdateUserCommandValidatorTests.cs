using FluentValidation.TestHelper;
using Shopizy.Application.Users.Commands.UpdateUser;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateUser;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    private static UpdateUserCommand ValidCommand() =>
        new(Guid.NewGuid(), "John", "Doe", "john@example.com", null, "123 Main St", "Springfield", "IL", "USA", "62701");

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_FirstNameIsEmpty()
    {
        var command = ValidCommand() with { FirstName = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Should_HaveError_When_LastNameIsEmpty()
    {
        var command = ValidCommand() with { LastName = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailIsInvalid()
    {
        var command = ValidCommand() with { Email = "not-an-email" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_EmailIsEmpty()
    {
        var command = ValidCommand() with { Email = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Should_HaveError_When_StreetIsEmpty()
    {
        var command = ValidCommand() with { Street = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllRequiredFieldsAreValid()
    {
        var command = ValidCommand();
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_PhoneNumberIsNull()
    {
        var command = ValidCommand() with { PhoneNumber = null };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public async Task Should_NotHaveError_When_PhoneNumberIsProvided()
    {
        var command = ValidCommand() with { PhoneNumber = "555-1234" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
