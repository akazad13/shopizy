using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface ICartRepository
{
    Task<IReadOnlyList<Cart>> GetCartsAsync();
    Task<Cart?> GetCartByIdAsync(CartId id, CancellationToken cancellationToken);
    Task<Cart?> GetCartByUserIdAsync(UserId id);
    Task<Cart?> GetCartByUserIdForUpdateAsync(UserId id);
    Task<IReadOnlyList<Cart>> GetAbandonedCartsAsync(
        DateTime inactiveBeforeUtc,
        int maxCount = 50,
        CancellationToken cancellationToken = default
    );
    Task AddAsync(Cart cart);
    void Update(Cart cart);
    void Remove(Cart cart);
}
