using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistance;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<List<Product>> GetProductsByIdsAsync(IEnumerable<ProductId> ids);
    Task<bool> IsProductExistAsync(ProductId id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    Task<int> Commit(CancellationToken cancellationToken);
}
