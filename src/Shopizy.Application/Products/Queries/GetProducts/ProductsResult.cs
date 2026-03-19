using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Queries.GetProducts;

public record ProductsResult(List<Product> Products, int TotalCount);
