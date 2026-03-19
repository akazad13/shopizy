using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Queries.GetWishlist;

public class GetWishlistQueryHandler(IWishlistRepository wishlistRepository)
    : IQueryHandler<GetWishlistQuery, ErrorOr<Wishlist>>
{
    public async Task<ErrorOr<Wishlist>> Handle(
        GetWishlistQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var wishlist = await wishlistRepository.GetWishlistByUserIdAsync(
            UserId.Create(query.UserId),
            cancellationToken
        );

        if (wishlist is null)
        {
            return CustomErrors.Wishlist.WishlistNotFound;
        }

        return wishlist;
    }
}
