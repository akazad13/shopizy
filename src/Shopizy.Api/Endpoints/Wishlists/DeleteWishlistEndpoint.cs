using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Commands.DeleteWishlist;
using Shopizy.Contracts.Common;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class DeleteWishlistEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete(
                "api/v1.0/users/{userId:guid}/wishlist",
                async (
                    Guid userId,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    ILogger<DeleteWishlistEndpoint> logger
                ) =>
                {
                    if (user.AuthorizeOwner(userId, "this wishlist") is { } forbidden)
                        return forbidden;

                    var command = new DeleteWishlistCommand(UserId.Create(userId));

                    return await HandleAsync(
                        mediator,
                        command,
                        _ => Results.NoContent(),
                        ex => logger.WishlistDeleteError(ex)
                    );
                }
            )
            .RequireAuthorization("Wishlist.Modify")
            .WithTags("Wishlist")
            .WithSummary("Delete wishlist")
            .WithDescription("Deletes the user's wishlist.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
