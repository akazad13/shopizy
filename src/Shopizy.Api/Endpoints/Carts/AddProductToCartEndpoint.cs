using System.Security.Claims;
using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Carts;

public class AddProductToCartEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/cart/items", async (Guid userId, AddProductToCartRequest request, ClaimsPrincipal user, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<AddProductToCartEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to modify this cart.")]);
            }

            var command = mapper.Map<AddProductToCartCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                cart => Results.Ok(mapper.Map<CartResponse>(cart)),
                ex => logger.CartCreationError(ex)
            );
        })
        .RequireAuthorization("Cart.Modify")
        .WithTags("Cart")
        .WithSummary("Add product to cart")
        .WithDescription("Adds a product or updates its quantity in the user's shopping cart.")
        .Produces<CartResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
