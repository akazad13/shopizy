using Shopizy.Domain.Products;

namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// DTO representing a faceted product search query.
/// </summary>
public record ProductSearchQueryDto(
    string? SearchTerm,
    IReadOnlyList<Guid>? CategoryIds = null,
    IReadOnlyList<Guid>? BrandIds = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? InStockOnly = null,
    decimal? MinRating = null,
    string? SortBy = null,
    int PageNumber = 1,
    int PageSize = 20
);

/// <summary>
/// DTO representing a single search result product item.
/// </summary>
public record ProductSearchResultItemDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    string? CategoryName,
    Guid? BrandId,
    string? BrandName,
    int StockQuantity,
    decimal AverageRating,
    int TotalReviews,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<string> Tags
);

/// <summary>
/// DTO representing a facet bucket value.
/// </summary>
public record FacetValueDto(string Key, string Label, int Count);

/// <summary>
/// DTO representing a facet category and its breakdown counts.
/// </summary>
public record SearchFacetDto(string FieldName, IReadOnlyList<FacetValueDto> Values);

/// <summary>
/// DTO representing full faceted search results including pagination, facets, and suggestions.
/// </summary>
public record ProductSearchResultDto(
    IReadOnlyList<ProductSearchResultItemDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    IReadOnlyList<SearchFacetDto> Facets,
    IReadOnlyList<string> SuggestedKeywords
);

/// <summary>
/// Engine interface for full-text, fuzzy, and faceted product searching.
/// </summary>
public interface IProductSearchEngine
{
    /// <summary>
    /// Executes a faceted and fuzzy search query across products.
    /// </summary>
    Task<ProductSearchResultDto> SearchProductsAsync(
        ProductSearchQueryDto query,
        CancellationToken cancellationToken = default
    );
}
