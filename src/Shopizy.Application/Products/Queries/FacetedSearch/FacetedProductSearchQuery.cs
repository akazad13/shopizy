using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Queries.FacetedSearch;

public record FacetedProductSearchQuery(
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
) : IQuery<ErrorOr<ProductSearchResultDto>>;
