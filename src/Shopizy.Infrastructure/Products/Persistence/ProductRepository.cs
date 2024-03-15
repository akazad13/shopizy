using Microsoft.EntityFrameworkCore;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace shopizy.Infrastructure.Products.Persistence;

public class ProductRepository(AppDbContext _dbContext) : IProductRepository
{
    public Task<List<Product>> GetProductsAsync()
    {
        return _dbContext.Products.AsNoTracking().ToListAsync();
    }
    public Task<Product?> GetProductByIdAsync(ProductId id)
    {
        return _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }
    public void Update(Product product)
    {
        _dbContext.Update(product);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}