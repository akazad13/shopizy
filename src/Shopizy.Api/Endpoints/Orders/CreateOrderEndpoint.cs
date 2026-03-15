using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Application.Common.Security.CurrentUser;
using Microsoft.AspNetCore.Mvc;

namespace Shopizy.Api.Endpoints.Orders;

public class CreateOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/orders/checkout", async (CreateOrderRequest request, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<CreateOrderEndpoint> logger) =>
        {
            var userId = currentUser.GetCurrentUserId();
            var command = mapper.Map<CreateOrderCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                order => Results.Ok(mapper.Map<OrderDetailResponse>(order)),
                ex => logger.OrderCreationError(ex)
            );
        })
        .RequireAuthorization("Order.Create")
        .WithTags("Orders")
        .WithSummary("Create order (Checkout)")
        .WithDescription("Processes a new order (Validate stock + Payment) for the authorized user.")
        .Produces<OrderDetailResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
