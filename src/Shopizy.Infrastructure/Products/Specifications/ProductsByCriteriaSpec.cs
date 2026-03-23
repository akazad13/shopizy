using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Products.Specifications;

internal class ProductsByCriteriaSpec : Specification<Product>
{
    public ProductsByCriteriaSpec(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly
    )
        : base(product =>
            (productIds == null || productIds.Contains(product.Id))
            && (name == null || product.Name.Contains(name))
            && (categoryIds == null || categoryIds.Contains(product.CategoryId))
            && (averageRating == null || averageRating <= product.AverageRating.Value)
            && (minPrice == null || product.UnitPrice.Amount >= minPrice)
            && (maxPrice == null || product.UnitPrice.Amount <= maxPrice)
            && (inStockOnly == null || inStockOnly == false || product.StockQuantity > 0)
        )
    {
        AddInclude(p => p.ProductImages);
    }
}
