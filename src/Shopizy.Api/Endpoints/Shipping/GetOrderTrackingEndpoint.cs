using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Shipping.Queries.GetOrderTracking;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Shipping;

public class GetOrderTrackingEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                "api/v1.0/orders/{orderId:guid}/tracking",
                async (
                    Guid orderId,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    ILogger<GetOrderTrackingEndpoint> logger
                ) =>
                {
                    var query = new GetOrderTrackingQuery(orderId);

                    return await HandleAsync(
                        mediator,
                        query,
                        tracking => Results.Ok(tracking),
                        ex => logger.OrderTrackingFetchError(ex)
                    );
                }
            )
            .RequireAuthorization()
            .WithTags("Shipping")
            .WithSummary("Get order shipment tracking")
            .WithDescription("Retrieves live tracking status and checkpoints for an order.")
            .Produces<ShippingTrackingInfoDto>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
