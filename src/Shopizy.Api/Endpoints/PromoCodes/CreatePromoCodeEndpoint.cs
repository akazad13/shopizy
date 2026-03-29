using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.PromoCodes.Commands.CreatePromoCode;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.PromoCode;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.PromoCodes;

public class CreatePromoCodeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/admin/promo-codes",
            async (
                CreatePromoCodeRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<CreatePromoCodeEndpoint> logger
            ) =>
            {
                var command = mapper.Map<CreatePromoCodeCommand>(request);

                return await HandleAsync(
                    mediator,
                    command,
                    promoCode => Results.Ok(mapper.Map<PromoCodeResponse>(promoCode)),
                    ex => logger.PromoCodeCreationError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.PromoCode.Create")
        .WithTags("PromoCodes")
        .WithSummary("Create promo code")
        .WithDescription("Creates a new promotional code.")
        .Produces<PromoCodeResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
