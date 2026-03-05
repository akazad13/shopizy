using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Carts;

public class GetCartEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}/carts", async (Guid userId, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetCartEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access this cart.")]);
            }

            var query = mapper.Map<GetCartQuery>(userId);

            return await HandleAsync(
                mediator,
                query,
                cart => Results.Ok(mapper.Map<CartResponse>(cart)),
                ex => logger.CartFetchError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Cart")
        .Produces<CartResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
