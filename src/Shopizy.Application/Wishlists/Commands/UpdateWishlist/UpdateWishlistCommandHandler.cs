using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.UpdateWishlist;

public class UpdateWishlistCommandHandler(
    IWishlistRepository wishlistRepository,
    IProductRepository productRepository
) : ICommandHandler<UpdateWishlistCommand, ErrorOr<Wishlist>>
{
    public async Task<ErrorOr<Wishlist>> Handle(
        UpdateWishlistCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var userId = UserId.Create(cmd.UserId);
        var productId = ProductId.Create(cmd.ProductId);

        var wishlist = await wishlistRepository.GetWishlistByUserIdAsync(userId, cancellationToken);
        if (wishlist is null)
            return CustomErrors.Wishlist.WishlistNotFound;

        if (!await productRepository.IsProductExistAsync(productId))
            return CustomErrors.Product.ProductNotFound;

        if (cmd.Action == WishlistAction.Add)
        {
            if (wishlist.WishlistItems.Any(i => i.ProductId == productId))
                return CustomErrors.Wishlist.ProductAlreadyInWishlist;

            wishlist.AddItem(productId);
        }
        else
        {
            if (!wishlist.WishlistItems.Any(i => i.ProductId == productId))
                return CustomErrors.Wishlist.ProductNotInWishlist;

            wishlist.RemoveItem(productId);
        }

        wishlistRepository.Update(wishlist);
        return wishlist;
    }
}
