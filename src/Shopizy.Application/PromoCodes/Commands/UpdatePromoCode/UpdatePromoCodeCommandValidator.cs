using FluentValidation;

namespace Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;

public class UpdatePromoCodeCommandValidator : AbstractValidator<UpdatePromoCodeCommand>
{
    public UpdatePromoCodeCommandValidator()
    {
        RuleFor(c => c.PromoCodeId).NotEmpty();
        RuleFor(c => c.Code).NotEmpty().MaximumLength(15);
        RuleFor(c => c.Description).MaximumLength(100);
        RuleFor(c => c.Discount).GreaterThan(0);
    }
}
