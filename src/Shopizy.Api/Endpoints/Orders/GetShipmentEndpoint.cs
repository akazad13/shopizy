using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Queries.GetShipment;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class GetShipmentEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/users/{userId:guid}/orders/{orderId:guid}/shipments",
            async (
                Guid userId,
                Guid orderId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetShipmentEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to access this shipment.")]
                    );
                }

                var query = new GetShipmentQuery(userId, orderId);

                return await HandleAsync(
                    mediator,
                    query,
                    shipment => Results.Ok(mapper.Map<ShipmentResponse>(shipment)),
                    ex => logger.OrderFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Order.Get")
        .WithTags("Orders")
        .WithSummary("Get shipment")
        .WithDescription("Retrieves the shipment details for a specific order.")
        .Produces<ShipmentResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
