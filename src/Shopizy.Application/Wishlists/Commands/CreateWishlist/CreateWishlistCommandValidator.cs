using FluentValidation;

namespace Shopizy.Application.Wishlists.Commands.CreateWishlist;

public class CreateWishlistCommandValidator : AbstractValidator<CreateWishlistCommand>
{
    public CreateWishlistCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}
