using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.UpdateShipment;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class UpdateShipmentEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/admin/orders/{id:guid}/shipments",
            async (
                Guid id,
                UpdateShipmentRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdateShipmentEndpoint> logger
            ) =>
            {
                var command = mapper.Map<UpdateShipmentCommand>((OrderId: id, req: request));

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Shipment updated successfully.")),
                    ex => logger.OrderStatusUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.UpdateOrderStatus")
        .WithTags("Orders")
        .WithSummary("Update shipment for order")
        .WithDescription("Allows an admin to update the shipment details for an existing order.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
