using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface ICartRepository
{
    Task<List<Cart>> GetCartsAsync();
    Task<Cart?> GetCartByIdAsync(CartId id);
    Task<Cart?> GetCartByUserIdAsync(UserId id);
    Task AddAsync(Cart cart);
    void Update(Cart cart);
    void Remove(Cart cart);
    Task<int> Commit(CancellationToken cancellationToken);
}
