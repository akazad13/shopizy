using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Orders;

public class GetOrdersEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}/orders", async (Guid userId, [AsParameters] OrdersCriteria ordersCriteria, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetOrdersEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access these orders.")]);
            }

            var query = mapper.Map<GetOrdersQuery>((userId, ordersCriteria));

            return await HandleAsync(
                mediator,
                query,
                orders => Results.Ok(mapper.Map<List<OrderResponse>>(orders)),
                ex => logger.OrderFetchError(ex)
            );
        })
        .RequireAuthorization("Order.Get")
        .WithTags("Orders")
        .WithSummary("List user orders")
        .WithDescription("Retrieves a history of all orders placed by the authorized user.")
        .Produces<List<OrderResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
