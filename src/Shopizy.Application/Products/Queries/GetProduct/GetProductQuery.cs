using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProduct;

/// <summary>
/// Represents a query to retrieve a product by its ID.
/// </summary>
/// <param name="ProductId">The product's unique identifier.</param>
public record GetProductQuery(Guid ProductId) : IQuery<ErrorOr<Product>>, ICachableRequest
{
    public string CacheKey => $"product-{ProductId}";
    public TimeSpan? Expiration => null;
}
