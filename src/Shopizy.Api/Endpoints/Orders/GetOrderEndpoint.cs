using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.Endpoints.Orders;

public class GetOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}/orders/{orderId:guid}", async (Guid userId, Guid orderId, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<GetOrderEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access this order.")]);
            }

            var query = mapper.Map<GetOrderQuery>((userId, orderId));

            return await HandleAsync(
                mediator,
                query,
                order => Results.Ok(mapper.Map<OrderDetailResponse>(order)),
                ex => logger.OrderFetchError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Orders")
        .Produces<OrderDetailResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
