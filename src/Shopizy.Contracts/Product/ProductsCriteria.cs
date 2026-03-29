namespace Shopizy.Contracts.Product;

/// <summary>
/// Represents criteria for searching and filtering products.
/// </summary>
/// <param name="ProductIds">Filter by product ids</param>
/// <param name="Name">Filter by product name.</param>
/// <param name="CategoryIds">Filter by category identifiers.</param>
/// <param name="AverageRating">Filter by minimum average rating.</param>
/// <param name="MinPrice">Filter by minimum price.</param>
/// <param name="MaxPrice">Filter by maximum price.</param>
/// <param name="InStockOnly">Filter to only in-stock products.</param>
/// <param name="SortBy">Sort order: "price_asc", "price_desc", "newest", "best_rated", "most_reviewed".</param>
/// <param name="PageNumber">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public record ProductsCriteria(
    Guid[]? ProductIds,
    string? Name,
    Guid[]? CategoryIds,
    decimal? AverageRating,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStockOnly,
    string? SortBy,
    int PageNumber = 1,
    int PageSize = 10
);
