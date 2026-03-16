using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Orders.Commands.UpdateOrderStatus;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Orders;

public class UpdateOrderStatusEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/admin/orders/{id:guid}/status", async (Guid id, [FromBody] Contracts.Order.OrderStatus status, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, ILogger<UpdateOrderStatusEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new UpdateOrderStatusCommand(currentUser.GetCurrentUserId(), id, (Domain.Orders.Enums.OrderStatus)(int)status),
                success => Results.Ok(SuccessResult.Success("Order status updated successfully.")),
                ex => logger.LogError(ex, "Error updating order status")
            );
        })
        .RequireAuthorization("Admin.UpdateOrderStatus")
        .WithTags("Orders")
        .WithSummary("Update order status")
        .WithDescription("Allows an admin to update the current tracking status of an order.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
