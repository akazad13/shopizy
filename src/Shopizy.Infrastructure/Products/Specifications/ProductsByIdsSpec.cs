using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Infrastructure.Products.Specifications;

internal class ProductsByIdsSpec : Specification<Product>
{
    public ProductsByIdsSpec(List<ProductId> ids) : base(product => ids.Contains(product.Id))
    {
        AddInclude(p => p.ProductImages);
    }
}
