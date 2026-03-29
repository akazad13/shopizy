using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Wishlists.Queries.GetPublicWishlist;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Wishlist;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Wishlists;

public class GetPublicWishlistEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/wishlists/{wishlistId:guid}",
            async (
                Guid wishlistId,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetPublicWishlistEndpoint> logger
            ) =>
            {
                var query = new GetPublicWishlistQuery(wishlistId);

                return await HandleAsync(
                    mediator,
                    query,
                    wishlist => Results.Ok(mapper.Map<WishlistResponse>(wishlist)),
                    ex => logger.WishlistFetchError(ex)
                );
            }
        )
        .AllowAnonymous()
        .WithTags("Wishlist")
        .WithSummary("Get public wishlist")
        .WithDescription("Retrieves a public wishlist by its ID. No authentication required.")
        .Produces<WishlistResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
