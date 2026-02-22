using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.Endpoints.Orders;

public class GetOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}/orders", async (Guid userId, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<GetOrdersEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access these orders.")]);
                }

                var query = mapper.Map<GetOrdersQuery>(userId);
                var result = await mediator.Send(query);

                return result.Match(
                    orders => Results.Ok(mapper.Map<List<OrderResponse>>(orders)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.OrderFetchError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .RequireAuthorization()
        .WithTags("Orders")
        .Produces<List<OrderResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
