using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistance;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task AddAsync(Product product);
    void Update(Product product);
    Task<int> Commit(CancellationToken cancellationToken);
}
