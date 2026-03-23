using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Commands.UpdateWishlistSettings;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class UpdateWishlistSettingsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/users/{userId:guid}/wishlist/settings",
            async (
                Guid userId,
                UpdateWishlistSettingsRequest request,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdateWishlistSettingsEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to update this wishlist.")]
                    );
                }

                var command = mapper.Map<UpdateWishlistSettingsCommand>((userId, request));

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
        .WithSummary("Update wishlist settings")
        .WithDescription("Updates the name and visibility settings for the user's wishlist.")
        .Produces<WishlistResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
