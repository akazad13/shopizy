using FluentValidation.TestHelper;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;

namespace Shopizy.Application.UnitTests.Payments.Commands.CashOnDeliverySale;

public class CashOnDeliverySaleCommandValidatorTests
{
    private readonly CashOnDeliverySaleCommandValidator _validator = new();

    private static CashOnDeliverySaleCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), 49.99m, "USD", "cod");

    [Fact]
    public async Task Should_HaveError_When_UserIdIsEmpty()
    {
        var command = ValidCommand() with { UserId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_HaveError_When_OrderIdIsEmpty()
    {
        var command = ValidCommand() with { OrderId = Guid.Empty };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Should_HaveError_When_AmountIsNotPositive(decimal amount)
    {
        var command = ValidCommand() with { Amount = amount };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public async Task Should_HaveError_When_CurrencyIsEmpty()
    {
        var command = ValidCommand() with { Currency = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public async Task Should_HaveError_When_CurrencyExceedsMaxLength()
    {
        var command = ValidCommand() with { Currency = new string('X', 11) };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public async Task Should_HaveError_When_PaymentMethodIsEmpty()
    {
        var command = ValidCommand() with { PaymentMethod = "" };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public async Task Should_HaveError_When_PaymentMethodExceedsMaxLength()
    {
        var command = ValidCommand() with { PaymentMethod = new string('X', 51) };
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public async Task Should_NotHaveErrors_When_AllFieldsAreValid()
    {
        var command = ValidCommand();
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
