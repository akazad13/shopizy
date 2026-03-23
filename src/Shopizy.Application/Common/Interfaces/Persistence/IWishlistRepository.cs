using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IWishlistRepository
{
    Task<Wishlist?> GetWishlistByUserIdAsync(UserId userId, CancellationToken cancellationToken);
    Task<Wishlist?> GetWishlistByIdAsync(WishlistId id, CancellationToken cancellationToken = default);
    Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken);
    void Update(Wishlist wishlist);
}
