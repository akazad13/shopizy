using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.GiftCards.Commands.RedeemGiftCard;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.GiftCard;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.GiftCards;

public class RedeemGiftCardEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/gift-cards/redeem",
                async (
                    RedeemGiftCardRequest request,
                    ClaimsPrincipal userClaims,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<RedeemGiftCardEndpoint> logger
                ) =>
                {
                    var userId = userClaims.GetUserId();
                    if (userId is null)
                    {
                        return Results.Unauthorized();
                    }

                    var command = new RedeemGiftCardCommand(userId, request.Code);

                    return await HandleAsync(
                        mediator,
                        command,
                        giftCard => Results.Ok(mapper.Map<GiftCardResponse>(giftCard)),
                        ex => logger.GiftCardFetchError(ex)
                    );
                }
            )
            .RequireAuthorization()
            .WithTags("GiftCards")
            .WithSummary("Redeem gift card")
            .WithDescription("Redeems an active gift card code for the authenticated user.")
            .Produces<GiftCardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
