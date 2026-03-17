using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IWishlistRepository
{
    Task<Wishlist?> GetWishlistByUserIdAsync(UserId userId, CancellationToken cancellationToken);
    Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken);
    void Update(Wishlist wishlist);
}
