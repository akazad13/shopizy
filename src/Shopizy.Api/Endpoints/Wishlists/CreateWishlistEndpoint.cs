using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Commands.CreateWishlist;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class CreateWishlistEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/users/{userId:guid}/wishlist",
            async (
                Guid userId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<CreateWishlistEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to create this wishlist.")]
                    );
                }

                var command = mapper.Map<CreateWishlistCommand>(userId);

                return await HandleAsync(
                    mediator,
                    command,
                    wishlist => Results.Created(
                        $"api/v1.0/users/{userId}/wishlist",
                        mapper.Map<WishlistResponse>(wishlist)
                    ),
                    ex => logger.WishlistCreationError(ex)
                );
            }
        )
        .RequireAuthorization("Wishlist.Create")
        .WithTags("Wishlist")
        .WithSummary("Create wishlist")
        .WithDescription("Creates a new wishlist for the authorized user.")
        .Produces<WishlistResponse>(StatusCodes.Status201Created)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
