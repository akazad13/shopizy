using FluentValidation;

namespace Shopizy.Application.PromoCodes.Commands.CreatePromoCode;

public class CreatePromoCodeCommandValidator : AbstractValidator<CreatePromoCodeCommand>
{
    public CreatePromoCodeCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(15);
        RuleFor(c => c.Description).MaximumLength(100);
        RuleFor(c => c.Discount).GreaterThan(0);
    }
}
