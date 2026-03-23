using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.PromoCode;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.PromoCodes;

public class UpdatePromoCodeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "api/v1.0/admin/promo-codes/{id:guid}",
            async (
                Guid id,
                UpdatePromoCodeRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdatePromoCodeEndpoint> logger
            ) =>
            {
                var command = mapper.Map<UpdatePromoCodeCommand>((id, request));

                return await HandleAsync(
                    mediator,
                    command,
                    promoCode => Results.Ok(mapper.Map<PromoCodeResponse>(promoCode)),
                    ex => logger.PromoCodeUpdateError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.PromoCode.Modify")
        .WithTags("PromoCodes")
        .WithSummary("Update promo code")
        .WithDescription("Activates, deactivates, or updates a promotional code.")
        .Produces<PromoCodeResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
