using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Carts.Persistence;

/// <summary>
/// Repository for managing cart data persistence.
/// </summary>
public class CartRepository(AppDbContext dbContext) : ICartRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves all carts from the database.
    /// </summary>
    /// <returns>A list of all carts.</returns>
    public Task<List<Cart>> GetCartsAsync()
    {
        return _dbContext.Carts.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Retrieves a cart by its unique identifier.
    /// </summary>
    /// <param name="id">The cart identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart if found; otherwise, null.</returns>
    public Task<Cart?> GetCartByIdAsync(CartId id, CancellationToken cancellationToken)
    {
        return _dbContext.Carts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a cart by user identifier, including cart items and products.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The cart if found; otherwise, null.</returns>
    public Task<Cart?> GetCartByUserIdAsync(UserId id)
    {
        return _dbContext
            .Carts.Include(c => c.CartItems)
            .ThenInclude(li => li.Product)
            .FirstOrDefaultAsync(c => c.UserId == id);
    }

    /// <summary>
    /// Adds a new cart to the database.
    /// </summary>
    /// <param name="cart">The cart to add.</param>
    public async Task AddAsync(Cart cart)
    {
        await _dbContext.Carts.AddAsync(cart);
    }

    /// <summary>
    /// Updates an existing cart in the database.
    /// </summary>
    /// <param name="cart">The cart to update.</param>
    public void Update(Cart cart)
    {
        _dbContext.Update(cart);
    }

    /// <summary>
    /// Removes a cart from the database.
    /// </summary>
    /// <param name="cart">The cart to remove.</param>
    public void Remove(Cart cart)
    {
        _dbContext.Remove(cart);
    }

    /// <summary>
    /// Commits all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
