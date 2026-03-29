using FluentValidation;

namespace Shopizy.Application.Payments.Commands.CashOnDeliverySale;

public class CashOnDeliverySaleCommandValidator : AbstractValidator<CashOnDeliverySaleCommand>
{
    public CashOnDeliverySaleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
    }
}
