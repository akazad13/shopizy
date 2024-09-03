using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Infrastructure.Products.Specifications;

namespace shopizy.Infrastructure.Products.Persistence;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<Product>> GetProductsAsync()
    {
        return _dbContext.Products.AsNoTracking().ToListAsync();
    }
    public Task<Product?> GetProductByIdAsync(ProductId id)
    {
        return _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
    public Task<List<Product>> GetProductsByIdsAsync(List<ProductId> ids)
    {
        return ApplySpec(new ProductsByIdsSpec(ids)).ToListAsync();
    }
    public Task<bool> IsProductExistAsync(ProductId id)
    {
        return _dbContext.Products.AnyAsync(p => p.Id == id);
    }
    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }
    public void Update(Product product)
    {
        _dbContext.Update(product);
    }

    public void Remove(Product product)
    {
        _dbContext.Remove(product);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Product> ApplySpec(Specification<Product> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Products, spec);
    }
}
