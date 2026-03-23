using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Orders;

public class CancelOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/users/{userId:guid}/orders/{orderId:guid}/cancel",
            async (
                Guid userId,
                Guid orderId,
                CancelOrderRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<CancelOrderEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to cancel this order.")]
                    );
                }

                var command = mapper.Map<CancelOrderCommand>((userId, orderId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Order cancelled successfully.")),
                    ex => logger.CancelOrderError(ex)
                );
            }
        )
        .RequireAuthorization("Order.Modify")
        .WithTags("Orders")
        .WithSummary("Cancel order")
        .WithDescription("Cancels an order belonging to the authenticated user.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
