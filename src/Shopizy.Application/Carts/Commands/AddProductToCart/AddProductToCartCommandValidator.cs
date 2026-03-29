using FluentValidation;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandValidator : AbstractValidator<AddProductToCartCommand>
{
    public AddProductToCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Size).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
