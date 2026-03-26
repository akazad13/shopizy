using FluentValidation;

namespace Shopizy.Application.LoyaltyAccounts.Commands.EarnPoints;

public class EarnPointsCommandValidator : AbstractValidator<EarnPointsCommand>
{
    public EarnPointsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Points).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
