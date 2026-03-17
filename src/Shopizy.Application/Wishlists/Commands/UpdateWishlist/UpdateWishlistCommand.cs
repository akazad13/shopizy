using ErrorOr;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.UpdateWishlist;

public enum WishlistAction
{
    Add,
    Remove,
}

public record UpdateWishlistCommand(Guid UserId, Guid ProductId, WishlistAction Action)
    : ICommand<ErrorOr<Wishlist>>;
