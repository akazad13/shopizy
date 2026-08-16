using FluentValidation;

namespace Shopizy.Application.GiftCards.Commands.RedeemGiftCard;

public class RedeemGiftCardCommandValidator : AbstractValidator<RedeemGiftCardCommand>
{
    public RedeemGiftCardCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Gift card code is required.");
        RuleFor(x => x.UserId).NotNull().WithMessage("User ID is required.");
    }
}
