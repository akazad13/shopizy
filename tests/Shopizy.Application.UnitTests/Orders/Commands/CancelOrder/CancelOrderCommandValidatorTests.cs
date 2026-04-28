using FluentValidation.TestHelper;
using Shopizy.Application.Orders.Commands.CancelOrder;

namespace Shopizy.Application.UnitTests.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidatorTests
{
    private readonly CancelOrderCommandValidator _validator = new();

    private static CancelOrderCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Customer request");

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_OrderIdIsEmpty()
    {
        var command = ValidCommand() with { OrderId = Guid.Empty };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public async Task Should_HaveError_When_ReasonIsEmpty()
    {
        var command = ValidCommand() with { Reason = "" };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public async Task Should_HaveError_When_ReasonExceedsMaxLength()
    {
        var command = ValidCommand() with { Reason = new string('A', 501) };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = ValidCommand();
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_NotHaveError_When_ReasonIsAtMaxLength()
    {
        var command = ValidCommand() with { Reason = new string('A', 500) };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldNotHaveValidationErrorFor(x => x.Reason);
    }
}
