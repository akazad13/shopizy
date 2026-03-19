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
        decimal? averageRating
    )
        : base(product =>
            (productIds == null || productIds.Contains(product.Id))
            && (name == null || product.Name.Contains(name))
            && (categoryIds == null || categoryIds.Contains(product.CategoryId))
            && (averageRating == null || averageRating <= product.AverageRating.Value)
        )
    {
        AddInclude(p => p.ProductImages);
    }
}
