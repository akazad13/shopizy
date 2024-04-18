using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Infrastructure.Products.Specifications;

internal class ProductsByIdsSpec(IEnumerable<ProductId> ids) : Specification<Product>(product => ids.Contains(product.Id))
{
}