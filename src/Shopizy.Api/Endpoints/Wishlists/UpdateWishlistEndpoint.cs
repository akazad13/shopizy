using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Commands.UpdateWishlist;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class UpdateWishlistEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/users/{userId:guid}/wishlist",
            async (
                Guid userId,
                UpdateWishlistRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdateWishlistEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to modify this wishlist.")]
                    );
                }

                var command = mapper.Map<UpdateWishlistCommand>((userId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    wishlist => Results.Ok(mapper.Map<WishlistResponse>(wishlist)),
                    ex => logger.WishlistUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("Wishlist.Update")
        .WithTags("Wishlist")
        .WithSummary("Update wishlist")
        .WithDescription("Adds or removes a product from the user's wishlist. Action must be \"Add\" or \"Remove\".")
        .Produces<WishlistResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
