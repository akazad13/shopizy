using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Carts.Persistence;

public class CartRepository(AppDbContext dbContext) : ICartRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<Cart>> GetCartsAsync()
    {
        return _dbContext.Carts.AsNoTracking().ToListAsync();
    }

    public Task<Cart?> GetCartByIdAsync(CartId id, CancellationToken cancellationToken)
    {
        return _dbContext.Carts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<Cart?> GetCartByUserIdAsync(UserId id)
    {
        return _dbContext
            .Carts.Include(c => c.CartItems)
            .ThenInclude(li => li.Product)
            .FirstOrDefaultAsync(c => c.UserId == id);
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
