using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Products.Specifications;

internal class ProductsByIdsSpec : Specification<Product>
{
    public ProductsByIdsSpec(List<ProductId> ids) : base(product => ids.Contains(product.Id))
    {
        AddInclude(p => p.ProductImages);
    }
}
