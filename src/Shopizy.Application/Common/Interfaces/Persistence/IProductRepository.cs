using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>?> GetProductsAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        IReadOnlyList<BrandId>? brandIds,
        string? color,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly,
        string? sortBy,
        int pageNumber,
        int pageSize
    );
    Task<int> CountProductsAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        IReadOnlyList<BrandId>? brandIds,
        string? color,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly
    );
    Task<(IReadOnlyList<Product> Products, int TotalCount)> GetProductsWithCountAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        IReadOnlyList<BrandId>? brandIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly,
        string? sortBy,
        int pageNumber,
        int pageSize
    );
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<Product?> GetProductByIdForUpdateAsync(ProductId id);
    Task<bool> IsProductExistAsync(ProductId id);
    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<ProductId> ids);
    Task<IReadOnlyList<Product>> GetProductsByIdsForUpdateAsync(IReadOnlyList<ProductId> ids);
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<int> GetTotalProductCountAsync();
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    void RemoveRange(IList<Product> products);
}

