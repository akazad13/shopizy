using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace shopizy.Application.Common.Interfaces.Persistance;

public interface IProductRepository
{
    Task<Product?> GetProductAsync(ProductId id);
    Task<List<Product>> GetProductsAsync();
    Task AddAsync(Product product);
    void Update(Product product);
    Task<int> Commit(CancellationToken cancellationToken);
}
