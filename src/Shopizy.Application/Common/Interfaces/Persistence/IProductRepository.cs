using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync(
        string? name,
        IList<CategoryId>? categoryIds,
        decimal? averageRating,
        int pageNumber,
        int pageSize
    );
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<List<Product>> GetProductsByIdsAsync(IList<ProductId> ids);
    Task<bool> IsProductExistAsync(ProductId id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
