using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<(IReadOnlyList<Product> Products, int TotalCount)> GetProductsWithCountAsync(
        ProductSearchCriteria criteria
    );
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<Product?> GetProductByIdForUpdateAsync(ProductId id);
    Task<bool> IsProductExistAsync(ProductId id);
    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<ProductId> ids);
    Task<IReadOnlyList<Product>> GetProductsByIdsForUpdateAsync(IReadOnlyList<ProductId> ids);
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    void RemoveRange(IList<Product> products);
}
