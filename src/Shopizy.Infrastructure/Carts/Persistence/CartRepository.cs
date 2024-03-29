using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Carts.Persistence;

public class CartRepository(AppDbContext _dbContext) : ICartRepository
{
    public Task<List<Cart>> GetCartsAsync()
    {
        return _dbContext.Carts.AsNoTracking().ToListAsync();
    }
    public Task<Cart?> GetCartByIdAsync(CartId id)
    {
        return _dbContext.Carts.FirstOrDefaultAsync(c => c.Id == id);
    }
    public Task<Cart?> GetCartByCustomerIdAsync(CustomerId id)
    {
        return _dbContext.Carts.FirstOrDefaultAsync(c => c.CustomerId == id);
    }
    public async Task AddAsync(Cart cart)
    {
        await _dbContext.Carts.AddAsync(cart);
    }
    public void Update(Cart cart)
    {
        _dbContext.Update(cart);
    }
    public void Remove(Cart cart)
    {
        _dbContext.Remove(cart);
    }
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}