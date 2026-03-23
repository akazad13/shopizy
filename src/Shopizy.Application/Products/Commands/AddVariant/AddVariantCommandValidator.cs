using FluentValidation;

namespace Shopizy.Application.Products.Commands.AddVariant;

public class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.SKU)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);
    }
}
