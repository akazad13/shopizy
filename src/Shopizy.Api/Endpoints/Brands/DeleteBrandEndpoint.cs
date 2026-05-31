using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Brands.Commands.DeleteBrand;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Brands;

public class DeleteBrandEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete(
                "api/v1.0/admin/brands/{brandId:guid}",
                async (
                    Guid brandId,
                    [FromServices] ICurrentUser currentUser,
                    [FromServices] IDispatcher mediator,
                    ILogger<DeleteBrandEndpoint> logger
                ) =>
                {
                    return await HandleAsync(
                        mediator,
                        new DeleteBrandCommand(currentUser.GetCurrentUserId(), brandId),
                        _ => Results.Ok(SuccessResult.Success("Successfully deleted brand.")),
                        ex => logger.BrandDeleteError(ex)
                    );
                }
            )
            .RequireAuthorization("Product.Delete")
            .WithTags("Brands")
            .WithSummary("Delete brand")
            .WithDescription("Deletes an existing brand.")
            .Produces<SuccessResult>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
