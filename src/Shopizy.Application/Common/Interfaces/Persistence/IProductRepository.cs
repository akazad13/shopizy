using System.Collections.Generic;
using System.Threading.Tasks;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>?> GetProductsAsync(
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        int pageNumber,
        int pageSize
    );
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<ProductId> ids);
    Task<bool> IsProductExistAsync(ProductId id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
}
