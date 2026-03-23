using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CreateShipment;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class CreateShipmentEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/admin/orders/{id:guid}/shipments",
            async (
                Guid id,
                CreateShipmentRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<CreateShipmentEndpoint> logger
            ) =>
            {
                var command = mapper.Map<CreateShipmentCommand>((id, request));

                return await HandleAsync(
                    mediator,
                    command,
                    shipment => Results.Created(
                        $"api/v1.0/admin/orders/{id}/shipments",
                        mapper.Map<ShipmentResponse>(shipment)
                    ),
                    ex => logger.OrderCreationError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.UpdateOrderStatus")
        .WithTags("Orders")
        .WithSummary("Create shipment")
        .WithDescription("Creates a new shipment for the specified order.")
        .Produces<ShipmentResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
