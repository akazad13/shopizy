using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Wishlists;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Wishlists.Persistence;

public class WishlistRepository(AppDbContext dbContext) : IWishlistRepository
{
    public async Task<Wishlist?> GetWishlistByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Wishlists.Include(w => w.WishlistItems)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken)
    {
        await dbContext.Wishlists.AddAsync(wishlist, cancellationToken);
    }

    public void Update(Wishlist wishlist)
    {
        dbContext.Update(wishlist);
    }
}
