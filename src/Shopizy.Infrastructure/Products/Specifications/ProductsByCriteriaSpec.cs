using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Products.Specifications;

internal class ProductsByCriteriaSpec : Specification<Product>
{
    public ProductsByCriteriaSpec(
        string? name,
        IList<CategoryId>? categoryIds,
        double? averageRating
    )
        : base(product =>
            (name == null || product.Name.Contains(name))
            && (categoryIds == null || categoryIds.Contains(product.CategoryId))
            && (averageRating == null || averageRating <= product.AverageRating.Value)
        )
    {
        AddInclude(p => p.ProductImages);
    }
}
