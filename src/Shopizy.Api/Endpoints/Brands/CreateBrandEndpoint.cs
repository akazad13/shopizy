using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Brands.Commands.CreateBrand;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Brands;

public class CreateBrandEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/v1.0/admin/brands",
                async (
                    [FromBody] CreateBrandRequest request,
                    [FromServices] ICurrentUser currentUser,
                    [FromServices] IDispatcher mediator,
                    ILogger<CreateBrandEndpoint> logger
                ) =>
                {
                    return await HandleAsync(
                        mediator,
                        new CreateBrandCommand(
                            currentUser.GetCurrentUserId(),
                            request.Name,
                            request.LogoUrl,
                            request.Country
                        ),
                        brand =>
                            Results.Ok(
                                new BrandResponse(
                                    brand.Id.Value,
                                    brand.Name,
                                    brand.LogoUrl,
                                    brand.Country
                                )
                            ),
                        ex => logger.BrandCreationError(ex)
                    );
                }
            )
            .RequireAuthorization("Product.Create")
            .WithTags("Brands")
            .WithSummary("Create brand")
            .WithDescription("Creates a new brand.")
            .Produces<BrandResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status409Conflict)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
