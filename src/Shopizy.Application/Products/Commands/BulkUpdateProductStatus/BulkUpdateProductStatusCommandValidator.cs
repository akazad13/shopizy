using FluentValidation;

namespace Shopizy.Application.Products.Commands.BulkUpdateProductStatus;

public class BulkUpdateProductStatusCommandValidator : AbstractValidator<BulkUpdateProductStatusCommand>
{
    public BulkUpdateProductStatusCommandValidator()
    {
        RuleFor(x => x.ProductIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one product ID must be provided.");

        RuleForEach(x => x.ProductIds)
            .NotEmpty()
            .WithMessage("Product IDs must not contain empty GUIDs.");
    }
}
