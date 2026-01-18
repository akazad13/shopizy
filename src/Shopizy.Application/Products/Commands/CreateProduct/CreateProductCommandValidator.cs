using FluentValidation;

namespace Shopizy.Application.Products.Commands.CreateProduct;

/// <summary>
/// Validator for the <see cref="CreateProductCommand"/>.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.ShortDescription)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(200);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Sku)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Brand)
            .MaximumLength(50);

        RuleFor(x => x.Barcode)
            .MaximumLength(50);
            
        RuleFor(x => x.Colors)
            .NotEmpty().MaximumLength(50);
            
        RuleFor(x => x.Sizes)
            .NotEmpty().MaximumLength(20);
            
        RuleFor(x => x.Tags)
            .MaximumLength(200);
    }
}
