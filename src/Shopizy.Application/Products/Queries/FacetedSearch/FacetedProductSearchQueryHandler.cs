using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Queries.FacetedSearch;

public class FacetedProductSearchQueryHandler(IProductSearchEngine searchEngine)
    : IQueryHandler<FacetedProductSearchQuery, ErrorOr<ProductSearchResultDto>>
{
    private readonly IProductSearchEngine _searchEngine = searchEngine;

    public async Task<ErrorOr<ProductSearchResultDto>> Handle(
        FacetedProductSearchQuery request,
        CancellationToken cancellationToken
    )
    {
        var searchDto = new ProductSearchQueryDto(
            request.SearchTerm,
            request.CategoryIds,
            request.BrandIds,
            request.MinPrice,
            request.MaxPrice,
            request.InStockOnly,
            request.MinRating,
            request.SortBy,
            request.PageNumber,
            request.PageSize
        );

        var result = await _searchEngine.SearchProductsAsync(searchDto, cancellationToken);
        return result;
    }
}
