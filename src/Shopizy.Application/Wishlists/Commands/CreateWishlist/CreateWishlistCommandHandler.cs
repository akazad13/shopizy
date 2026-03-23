using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.CreateWishlist;

public class CreateWishlistCommandHandler(IWishlistRepository wishlistRepository)
    : ICommandHandler<CreateWishlistCommand, ErrorOr<Wishlist>>
{
    public async Task<ErrorOr<Wishlist>> Handle(
        CreateWishlistCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var userId = UserId.Create(cmd.UserId);

        var existing = await wishlistRepository.GetWishlistByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return CustomErrors.Wishlist.WishlistAlreadyExists;
        }

        var wishlist = Wishlist.Create(userId, cmd.Name, cmd.IsPublic);
        await wishlistRepository.AddAsync(wishlist, cancellationToken);

        return wishlist;
    }
}
