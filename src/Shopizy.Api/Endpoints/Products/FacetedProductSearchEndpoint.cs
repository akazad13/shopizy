using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Queries.FacetedSearch;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

public record FacetedProductSearchRequest(
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

public class FacetedProductSearchEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/products/faceted-search",
                async (
                    [FromBody] FacetedProductSearchRequest request,
                    [FromServices] IDispatcher mediator,
                    ILogger<FacetedProductSearchEndpoint> logger
                ) =>
                {
                    var query = new FacetedProductSearchQuery(
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

                    return await HandleAsync(
                        mediator,
                        query,
                        results => Results.Ok(results),
                        ex => logger.ProductSearchError(ex)
                    );
                }
            )
            .AllowAnonymous()
            .WithTags("Products")
            .WithSummary("Faceted product search")
            .WithDescription(
                "Executes fuzzy tokenized product search returning dynamic faceted category/brand/price/rating breakdown counts."
            )
            .Produces<ProductSearchResultDto>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
