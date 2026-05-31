using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.PromoCodes.Queries.ValidatePromoCode;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.PromoCode;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.PromoCodes;

public class ValidatePromoCodeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/users/{userId:guid}/orders/validate-promo",
                async (
                    Guid userId,
                    [FromBody] string code,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<ValidatePromoCodeEndpoint> logger
                ) =>
                {
                    if (user.AuthorizeOwner(userId, "this user's promo codes") is { } forbidden)
                        return forbidden;

                    return await HandleAsync(
                        mediator,
                        new ValidatePromoCodeQuery(code),
                        promoCode => Results.Ok(mapper.Map<PromoCodeResponse>(promoCode)),
                        ex => logger.PromoCodeFetchError(ex)
                    );
                }
            )
            .RequireAuthorization("Order.Create")
            .WithTags("PromoCodes")
            .WithSummary("Validate a promo code")
            .WithDescription("Validates a promo code and returns the discount details if active.")
            .Produces<PromoCodeResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status404NotFound)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
