using Microsoft.EntityFrameworkCore;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace shopizy.Infrastructure.Products.Persistence;

public class ProductRepository(AppDbContext _dbContext) : IProductRepository
{
    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Product?> GetProductAsync(ProductId id)
    {
        return _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<List<Product>> GetProductsAsync()
    {
        return _dbContext.Products.AsNoTracking().ToListAsync();
    }

    public void Update(Product product)
    {
        _dbContext.Update(product);
    }
}