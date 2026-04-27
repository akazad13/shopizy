using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public record ProductSearchCriteria(
    IReadOnlyList<ProductId>? ProductIds,
    string? Name,
    IReadOnlyList<CategoryId>? CategoryIds,
    IReadOnlyList<BrandId>? BrandIds,
    decimal? AverageRating,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStockOnly,
    string? SortBy,
    int PageNumber = 1,
    int PageSize = 10
);
