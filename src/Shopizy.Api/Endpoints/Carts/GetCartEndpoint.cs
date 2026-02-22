using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Carts;

public class GetCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/users/{userId:guid}/carts", async (Guid userId, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<GetCartEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to access this cart.")]);
                }

                var query = mapper.Map<GetCartQuery>(userId);
                var result = await mediator.Send(query);

                return result.Match(
                    cart => Results.Ok(mapper.Map<CartResponse>(cart)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CartFetchError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
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
