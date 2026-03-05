using System.Collections.Generic;
using System.Threading.Tasks;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface ICartRepository
{
    Task<IReadOnlyList<Cart>> GetCartsAsync();
    Task<Cart?> GetCartByIdAsync(CartId id, CancellationToken cancellationToken);
    Task<Cart?> GetCartByUserIdAsync(UserId id);
    Task AddAsync(Cart cart);
    void Update(Cart cart);
    void Remove(Cart cart);
}
