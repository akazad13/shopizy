using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Brands.Queries.GetBrand;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Brands;

public class GetBrandEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/brands/{brandId:guid}", async (
            Guid brandId,
            [FromServices] IDispatcher mediator,
            ILogger<GetBrandEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetBrandQuery(brandId),
                brand => Results.Ok(new BrandResponse(brand.Id.Value, brand.Name, brand.LogoUrl, brand.Country)),
                ex => logger.BrandFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Brands")
        .WithSummary("Get brand by ID")
        .WithDescription("Retrieves brand details by identifier.")
        .Produces<BrandResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}