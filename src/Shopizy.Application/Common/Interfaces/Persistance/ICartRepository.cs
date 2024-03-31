using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistance;

public interface ICartRepository
{
    Task<List<Cart>> GetCartsAsync();
    Task<Cart?> GetCartByIdAsync(CartId id);
    Task<Cart?> GetCartByCustomerIdAsync(CustomerId id);
    Task AddAsync(Cart cart);
    void Update(Cart cart);
    void Remove(Cart cart);
    Task<int> Commit(CancellationToken cancellationToken);
}
