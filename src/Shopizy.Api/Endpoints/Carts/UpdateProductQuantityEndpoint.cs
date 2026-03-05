using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Carts;

public class UpdateProductQuantityEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/carts/{cartId:guid}/items/{itemId:guid}", async (Guid userId, Guid cartId, Guid itemId, UpdateProductQuantityRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<UpdateProductQuantityEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to update this cart.")]);
            }

            var command = mapper.Map<UpdateProductQuantityCommand>((userId, cartId, itemId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated cart.")),
                ex => logger.CartUpdateError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Cart")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
