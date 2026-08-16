using FluentValidation.TestHelper;
using Shopizy.Application.PromoCodes.Commands.CreatePromoCode;

namespace Shopizy.Application.UnitTests.PromoCodes.Commands.CreatePromoCode;

public class CreatePromoCodeCommandValidatorTests
{
    private readonly CreatePromoCodeCommandValidator _validator = new();

    [Fact]
    public void Should_PassValidation_WhenCommandIsValid()
    {
        var command = new CreatePromoCodeCommand("PROMO10", "10% Off", 10, true, true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenCodeIsEmpty()
    {
        var command = new CreatePromoCodeCommand("", "10% Off", 10, true, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_HaveError_WhenDiscountIsZeroOrNegative()
    {
        var command = new CreatePromoCodeCommand("PROMO10", "10% Off", 0, true, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }
}
