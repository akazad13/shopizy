using FluentValidation.TestHelper;
using Shopizy.Application.Products.Commands.AddVariant;
using Xunit;

namespace Shopizy.Application.UnitTests.Products.Commands.AddVariant;

public class AddVariantCommandValidatorTests
{
    private readonly AddVariantCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var command = new AddVariantCommand(
            Guid.NewGuid(),
            "Variant A",
            "SKU123",
            29.99m,
            Shopizy.Domain.Common.Enums.Currency.usd,
            50
        );
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenCommandHasInvalidFields_ShouldHaveValidationErrors()
    {
        var command = new AddVariantCommand(
            Guid.Empty,
            "",
            "",
            0m,
            Shopizy.Domain.Common.Enums.Currency.usd,
            -1
        );
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.SKU);
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }
}
