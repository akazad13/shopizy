using FluentValidation;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

public class RemoveProductFromCartCommandValidator : AbstractValidator<RemoveProductFromCartCommand>
{
    public RemoveProductFromCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}
