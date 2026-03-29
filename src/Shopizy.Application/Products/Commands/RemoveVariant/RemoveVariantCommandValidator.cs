using FluentValidation;

namespace Shopizy.Application.Products.Commands.RemoveVariant;

public class RemoveVariantCommandValidator : AbstractValidator<RemoveVariantCommand>
{
    public RemoveVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VariantId).NotEmpty();
    }
}
