using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.RemoveWishlistItem;

public class RemoveWishlistItemCommandHandler(IWishlistRepository wishlistRepository)
    : ICommandHandler<RemoveWishlistItemCommand, ErrorOr<Wishlist>>
{
    private readonly IWishlistRepository _wishlistRepository = wishlistRepository;

    public async Task<ErrorOr<Wishlist>> Handle(
        RemoveWishlistItemCommand request,
        CancellationToken cancellationToken
    )
    {
        var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(
            request.UserId,
            cancellationToken
        );

        if (wishlist is null)
        {
            return (Error)CustomErrors.Wishlist.WishlistNotFound;
        }

        wishlist.RemoveItem(request.ProductId);
        _wishlistRepository.Update(wishlist);

        return wishlist;
    }
}
