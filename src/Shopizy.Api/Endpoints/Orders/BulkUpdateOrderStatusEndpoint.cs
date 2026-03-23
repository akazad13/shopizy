using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class BulkUpdateOrderStatusEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/admin/orders/bulk-status", async (
            [FromBody] BulkUpdateOrderStatusRequest request,
            [FromServices] IDispatcher mediator,
            ILogger<BulkUpdateOrderStatusEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new BulkUpdateOrderStatusCommand(request.OrderIds, request.Status),
                _ => Results.Ok(SuccessResult.Success("Successfully updated order statuses.")),
                ex => logger.OrderStatusUpdateError(ex)
            );
        })
        .RequireAuthorization("Admin.UpdateOrderStatus")
        .WithTags("Orders")
        .WithSummary("Bulk update order status")
        .WithDescription("Updates the status for multiple orders.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
