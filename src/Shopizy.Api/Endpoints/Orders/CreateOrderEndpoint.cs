using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.Endpoints.Orders;

public class CreateOrderEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/orders", async (Guid userId, CreateOrderRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<CreateOrderEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to create an order for this user.")]);
            }

            var command = mapper.Map<CreateOrderCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                order => Results.Ok(mapper.Map<OrderDetailResponse>(order)),
                ex => logger.OrderCreationError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Orders")
        .Produces<OrderDetailResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
