using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Queries.GetPublicWishlist;

public class GetPublicWishlistQueryHandler(IWishlistRepository wishlistRepository)
    : IQueryHandler<GetPublicWishlistQuery, ErrorOr<Wishlist>>
{
    public async Task<ErrorOr<Wishlist>> Handle(
        GetPublicWishlistQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var wishlist = await wishlistRepository.GetWishlistByIdAsync(
            WishlistId.Create(request.WishlistId),
            cancellationToken
        );

        if (wishlist is null)
        {
            return CustomErrors.Wishlist.WishlistNotFound;
        }

        if (!wishlist.IsPublic)
        {
            return Error.Forbidden("Wishlist.NotPublic", "This wishlist is not public.");
        }

        return wishlist;
    }
}
