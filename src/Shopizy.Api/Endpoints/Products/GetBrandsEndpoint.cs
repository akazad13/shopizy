using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Brands.Queries.ListBrands;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

public class GetBrandsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                "api/v1.0/brands",
                async ([FromServices] IDispatcher mediator, ILogger<GetBrandsEndpoint> logger) =>
                {
                    return await HandleAsync(
                        mediator,
                        new ListBrandsQuery(),
                        brands =>
                            Results.Ok(
                                brands.Select(b => new BrandResponse(
                                    b.Id,
                                    b.Name,
                                    b.LogoUrl,
                                    b.Country
                                ))
                            ),
                        ex => logger.BrandsFetchError(ex)
                    );
                }
            )
            .AllowAnonymous()
            .WithTags("Brands")
            .WithSummary("Get all brands")
            .WithDescription("Retrieves a list of all brands available in the catalog.")
            .Produces<List<BrandResponse>>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
