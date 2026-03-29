using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Queries.GetWishlist;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class GetWishlistEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/users/{userId:guid}/wishlist",
            async (
                Guid userId,
                ClaimsPrincipal user,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetWishlistEndpoint> logger
            ) =>
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem(
                        [ErrorOr.Error.Forbidden(description: "You are not authorized to access this wishlist.")]
                    );
                }

                var query = mapper.Map<GetWishlistQuery>(userId);

                return await HandleAsync(
                    mediator,
                    query,
                    wishlist => Results.Ok(mapper.Map<WishlistResponse>(wishlist)),
                    ex => logger.WishlistFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Wishlist.Get")
        .WithTags("Wishlist")
        .WithSummary("Get wishlist")
        .WithDescription("Retrieves the wishlist for the authorized user.")
        .Produces<WishlistResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
