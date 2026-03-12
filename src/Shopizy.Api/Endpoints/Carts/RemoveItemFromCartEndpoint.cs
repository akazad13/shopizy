using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Carts;

public class RemoveItemFromCartEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/carts/{cartId:guid}/items/{itemId:guid}", async (Guid userId, Guid cartId, Guid itemId, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<RemoveItemFromCartEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to remove items from this cart.")]);
            }

            var command = mapper.Map<RemoveProductFromCartCommand>((userId, cartId, itemId));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully removed product from cart.")),
                ex => logger.RemoveItemFromCartError(ex)
            );
        })
        .RequireAuthorization()
        .WithTags("Cart")
        .WithSummary("Remove item from cart")
        .WithDescription("Deletes a specific item from the user's shopping cart.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
