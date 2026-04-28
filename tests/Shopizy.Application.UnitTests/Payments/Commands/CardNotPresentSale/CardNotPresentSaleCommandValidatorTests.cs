using FluentValidation.TestHelper;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;

namespace Shopizy.Application.UnitTests.Payments.Commands.CardNotPresentSale;

public class CardNotPresentSaleCommandValidatorTests
{
    private readonly CardNotPresentSaleCommandValidator _validator = new();

    private static CardNotPresentSaleCommand ValidCommand() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            99.99m,
            "USD",
            "card",
            "pm_123",
            "John Doe",
            12,
            2030,
            "4242"
        );

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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_HaveError_When_AmountIsNotPositive(decimal amount)
    {
        var command = ValidCommand() with { Amount = amount };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public async Task Should_HaveError_When_CurrencyIsEmpty()
    {
        var command = ValidCommand() with { Currency = "" };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public async Task Should_HaveError_When_CurrencyExceedsMaxLength()
    {
        var command = ValidCommand() with { Currency = new string('X', 11) };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public async Task Should_HaveError_When_PaymentMethodIsEmpty()
    {
        var command = ValidCommand() with { PaymentMethod = "" };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public async Task Should_HaveError_When_PaymentMethodExceedsMaxLength()
    {
        var command = ValidCommand() with { PaymentMethod = new string('X', 51) };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public async Task Should_HaveError_When_PaymentMethodIdIsEmpty()
    {
        var command = ValidCommand() with { PaymentMethodId = "" };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
    }

    [Fact]
    public async Task Should_HaveError_When_CardNameExceedsMaxLength()
    {
        var command = ValidCommand() with { CardName = new string('A', 101) };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.CardName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(13)]
    public async Task Should_HaveError_When_CardExpiryMonthIsOutOfRange(int month)
    {
        var command = ValidCommand() with { CardExpiryMonth = month };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.CardExpiryMonth);
    }

    [Fact]
    public async Task Should_NotHaveError_When_CardExpiryMonthIsZero()
    {
        // 0 means "not provided" — the When condition skips the rule
        var command = ValidCommand() with
        {
            CardExpiryMonth = 0,
        };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldNotHaveValidationErrorFor(x => x.CardExpiryMonth);
    }

    [Fact]
    public async Task Should_HaveError_When_CardExpiryYearIsNegative()
    {
        var command = ValidCommand() with { CardExpiryYear = -1 };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.CardExpiryYear);
    }

    [Fact]
    public async Task Should_NotHaveError_When_CardExpiryYearIsZero()
    {
        // 0 means "not provided" — the When condition skips the rule
        var command = ValidCommand() with
        {
            CardExpiryYear = 0,
        };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldNotHaveValidationErrorFor(x => x.CardExpiryYear);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public async Task Should_HaveError_When_LastDigitsIsNotFourCharacters(string lastDigits)
    {
        var command = ValidCommand() with { LastDigits = lastDigits };
        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.ShouldHaveValidationErrorFor(x => x.LastDigits);
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
}
