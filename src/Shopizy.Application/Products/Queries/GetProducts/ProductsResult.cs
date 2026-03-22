using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public record ProductsResult(IReadOnlyList<Product> Products, int TotalCount);
