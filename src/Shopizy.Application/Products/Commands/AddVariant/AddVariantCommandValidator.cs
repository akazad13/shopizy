using FluentValidation;

namespace Shopizy.Application.Products.Commands.AddVariant;

public class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.SKU).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
