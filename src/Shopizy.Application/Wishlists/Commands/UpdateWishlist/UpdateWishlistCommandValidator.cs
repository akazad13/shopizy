using FluentValidation;

namespace Shopizy.Application.Wishlists.Commands.UpdateWishlist;

public class UpdateWishlistCommandValidator : AbstractValidator<UpdateWishlistCommand>
{
    public UpdateWishlistCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.Action).IsInEnum();
    }
}
