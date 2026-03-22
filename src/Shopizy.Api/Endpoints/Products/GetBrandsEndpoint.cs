using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Products.Queries.GetBrands;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;

namespace Shopizy.Api.Endpoints.Products;

public class GetBrandsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/brands", async ([FromServices] IDispatcher mediator, ILogger<GetBrandsEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetBrandsQuery(),
                brands => Results.Ok(brands.Select(b => new BrandResponse(b))),
                ex => logger.BrandsFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Products")
        .WithSummary("Get all brands")
        .WithDescription("Retrieves a list of all unique product brands available in the catalog.")
        .Produces<List<BrandResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
