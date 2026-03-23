using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;

namespace Shopizy.Application.Products.Queries.GetProducts;

/// <summary>
/// Represents a query to retrieve a paginated list of products with optional filtering.
/// </summary>
/// <param name="ProductIds">Filter by product ids</param>
/// <param name="Name">Optional product name filter.</param>
/// <param name="CategoryIds">Optional list of category IDs to filter by.</param>
/// <param name="AverageRating">Optional minimum average rating filter.</param>
/// <param name="MinPrice">Optional minimum price filter.</param>
/// <param name="MaxPrice">Optional maximum price filter.</param>
/// <param name="InStockOnly">Optional filter to only in-stock products.</param>
/// <param name="SortBy">Optional sort order.</param>
/// <param name="PageNumber">The page number.</param>
/// <param name="PageSize">The page size.</param>
public record GetProductsQuery(
    IReadOnlyList<Guid>? ProductIds,
    string? Name,
    IList<Guid>? CategoryIds,
    decimal? AverageRating,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStockOnly,
    string? SortBy,
    int PageNumber,
    int PageSize
) : IQuery<ErrorOr<ProductsResult>>, ICachableRequest
{
    public string CacheKey
    {
        get
        {
            var categoryIdsStr = CategoryIds != null ? string.Join(",", CategoryIds) : "none";
            var productIdsStr = ProductIds != null ? string.Join(",", ProductIds): "none";
            return $"products-ids:{productIdsStr}-name:{Name}-categories:{categoryIdsStr}-rating:{AverageRating}-minPrice:{MinPrice}-maxPrice:{MaxPrice}-inStock:{InStockOnly}-sortBy:{SortBy}-page:{PageNumber}-size:{PageSize}";
        }
    }

    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
