using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Commands.RemoveWishlistItem;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class RemoveWishlistItemEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete(
                "api/v1.0/users/{userId:guid}/wishlist/items/{productId:guid}",
                async (
                    [FromRoute] Guid userId,
                    [FromRoute] Guid productId,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<RemoveWishlistItemEndpoint> logger
                ) =>
                {
                    if (user.AuthorizeOwner(userId, "this wishlist") is { } forbidden)
                        return forbidden;

                    var command = new RemoveWishlistItemCommand(
                        UserId.Create(userId),
                        ProductId.Create(productId)
                    );

                    return await HandleAsync(
                        mediator,
                        command,
                        wishlist => Results.Ok(mapper.Map<WishlistResponse>(wishlist)),
                        ex => logger.WishlistUpdateError(ex)
                    );
                }
            )
            .RequireAuthorization("Wishlist.Modify")
            .WithTags("Wishlist")
            .WithSummary("Remove wishlist item")
            .WithDescription("Removes a product from the user's wishlist.")
            .Produces<WishlistResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
