using FluentValidation;

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

public class CardNotPresentSaleCommandValidator : AbstractValidator<CardNotPresentSaleCommand>
{
    public CardNotPresentSaleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PaymentMethodId).NotEmpty();
        RuleFor(x => x.CardName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CardExpiryMonth).InclusiveBetween(1, 12);
        RuleFor(x => x.CardExpiryYear).GreaterThan(0);
        RuleFor(x => x.LastDigits).NotEmpty().Length(4);
    }
}
