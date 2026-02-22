using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Carts;

public class AddProductToCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/carts/{cartId:guid}", async (Guid userId, Guid cartId, AddProductToCartRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<AddProductToCartEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to modify this cart.")]);
                }

                var command = mapper.Map<AddProductToCartCommand>((userId, cartId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    cart => Results.Ok(mapper.Map<CartResponse>(cart)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CartCreationError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .RequireAuthorization()
        .WithTags("Cart")
        .Produces<CartResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
