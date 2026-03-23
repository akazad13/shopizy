using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.UpdateWishlistSettings;

public class UpdateWishlistSettingsCommandHandler(IWishlistRepository wishlistRepository)
    : ICommandHandler<UpdateWishlistSettingsCommand, ErrorOr<Wishlist>>
{
    public async Task<ErrorOr<Wishlist>> Handle(
        UpdateWishlistSettingsCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var wishlist = await wishlistRepository.GetWishlistByUserIdAsync(
            UserId.Create(request.UserId),
            cancellationToken
        );

        if (wishlist is null)
        {
            return CustomErrors.Wishlist.WishlistNotFound;
        }

        wishlist.UpdateSettings(request.Name, request.IsPublic);
        wishlistRepository.Update(wishlist);

        return wishlist;
    }
}
