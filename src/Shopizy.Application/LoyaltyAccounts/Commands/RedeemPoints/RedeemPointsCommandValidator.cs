using FluentValidation;

namespace Shopizy.Application.LoyaltyAccounts.Commands.RedeemPoints;

public class RedeemPointsCommandValidator : AbstractValidator<RedeemPointsCommand>
{
    public RedeemPointsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Points).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
