using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Wishlists.Commands.DeleteWishlist;

public class DeleteWishlistCommandHandler(IWishlistRepository wishlistRepository)
    : ICommandHandler<DeleteWishlistCommand, ErrorOr<Success>>
{
    private readonly IWishlistRepository _wishlistRepository = wishlistRepository;

    public async Task<ErrorOr<Success>> Handle(
        DeleteWishlistCommand request,
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

        _wishlistRepository.Remove(wishlist);

        return Result.Success;
    }
}
