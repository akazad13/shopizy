using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Brands.Commands.UpdateBrand;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Brands;

public class UpdateBrandEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/admin/brands/{brandId:guid}", async (
            Guid brandId,
            [FromBody] UpdateBrandRequest request,
            [FromServices] ICurrentUser currentUser,
            [FromServices] IDispatcher mediator,
            ILogger<UpdateBrandEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new UpdateBrandCommand(
                    currentUser.GetCurrentUserId(),
                    brandId,
                    request.Name,
                    request.LogoUrl,
                    request.Country
                ),
                _ => Results.Ok(SuccessResult.Success("Successfully updated brand.")),
                ex => logger.BrandUpdateError(ex)
            );
        })
        .RequireAuthorization("Product.Modify")
        .WithTags("Brands")
        .WithSummary("Update brand")
        .WithDescription("Updates an existing brand.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}