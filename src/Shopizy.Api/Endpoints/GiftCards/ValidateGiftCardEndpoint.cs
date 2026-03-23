using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.GiftCards.Commands.ValidateGiftCard;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.GiftCard;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.GiftCards;

public class ValidateGiftCardEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/gift-cards/validate",
            async (
                [FromBody] string code,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<ValidateGiftCardEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new ValidateGiftCardCommand(code),
                    giftCard => Results.Ok(mapper.Map<GiftCardResponse>(giftCard)),
                    ex => logger.GiftCardFetchError(ex)
                );
            }
        )
        .RequireAuthorization()
        .WithTags("GiftCards")
        .WithSummary("Validate gift card")
        .WithDescription("Validates a gift card code and returns details if active.")
        .Produces<GiftCardResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
