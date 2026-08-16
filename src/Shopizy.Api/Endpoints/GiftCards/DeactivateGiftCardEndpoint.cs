using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.GiftCards.Commands.DeactivateGiftCard;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.GiftCard;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.GiftCards;

public class DeactivateGiftCardEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPatch(
                "api/v1.0/admin/gift-cards/{id:guid}/deactivate",
                async (
                    [FromRoute] Guid id,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<DeactivateGiftCardEndpoint> logger
                ) =>
                {
                    var command = new DeactivateGiftCardCommand(GiftCardId.Create(id));

                    return await HandleAsync(
                        mediator,
                        command,
                        giftCard => Results.Ok(mapper.Map<GiftCardResponse>(giftCard)),
                        ex => logger.GiftCardFetchError(ex)
                    );
                }
            )
            .RequireAuthorization("Admin")
            .WithTags("GiftCards")
            .WithSummary("Deactivate gift card")
            .WithDescription("Deactivates an existing gift card.")
            .Produces<GiftCardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
