using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CreateShipment;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class AddShipmentEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/admin/orders/{orderId:guid}/shipment",
            async (
                Guid orderId,
                [FromBody] CreateShipmentRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<AddShipmentEndpoint> logger
            ) =>
            {
                var command = mapper.Map<CreateShipmentCommand>((OrderId: orderId, req: request));

                return await HandleAsync(
                    mediator,
                    command,
                    shipment => Results.Created(
                        $"api/v1.0/admin/orders/{orderId}/shipment",
                        mapper.Map<ShipmentResponse>(shipment)
                    ),
                    ex => logger.OrderFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Admin")
        .WithTags("Orders")
        .WithSummary("Add shipment to order")
        .WithDescription("Allows an admin to add a shipment to an existing order.")
        .Produces<ShipmentResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
