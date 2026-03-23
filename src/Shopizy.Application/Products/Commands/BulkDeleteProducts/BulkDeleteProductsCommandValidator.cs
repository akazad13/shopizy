using FluentValidation;

namespace Shopizy.Application.Products.Commands.BulkDeleteProducts;

public class BulkDeleteProductsCommandValidator : AbstractValidator<BulkDeleteProductsCommand>
{
    public BulkDeleteProductsCommandValidator()
    {
        RuleFor(x => x.ProductIds)
            .NotEmpty()
            .WithMessage("At least one product ID must be provided.");
    }
}
