using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Admin.Queries.GetTopProducts;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Admin;

public class GetTopProductsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/admin/reports/products/top", async (
            [FromQuery] int count,
            [FromServices] IDispatcher mediator,
            IMapper mapper,
            ILogger<GetTopProductsEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new GetTopProductsQuery(count),
                products => Results.Ok(mapper.Map<List<TopProductResponse>>(products)),
                ex => logger.ProductFetchError(ex)
            );
        })
        .RequireAuthorization("Admin.Reports")
        .WithTags("Admin Reports")
        .WithSummary("Get top products by revenue")
        .WithDescription("Retrieves the top products ranked by revenue.")
        .Produces<List<TopProductResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
