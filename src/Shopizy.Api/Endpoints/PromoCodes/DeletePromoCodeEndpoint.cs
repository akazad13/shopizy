using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.PromoCodes.Commands.DeletePromoCode;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.PromoCodes;

public class DeletePromoCodeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "api/v1.0/admin/promo-codes/{id:guid}",
            async (
                Guid id,
                [FromServices] IDispatcher mediator,
                ILogger<DeletePromoCodeEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new DeletePromoCodeCommand(id),
                    _ => Results.Ok(SuccessResult.Success("Promo code deleted successfully.")),
                    ex => logger.PromoCodeDeleteError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.PromoCode.Delete")
        .WithTags("PromoCodes")
        .WithSummary("Delete promo code")
        .WithDescription("Permanently removes a promotional code.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
