using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.RemoveVariant;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

/// <summary>
/// Endpoint for removing a product variant.
/// </summary>
public class RemoveVariantEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "api/v1.0/admin/products/{productId:guid}/variants/{variantId:guid}",
            async (
                Guid productId,
                Guid variantId,
                [FromServices] IDispatcher mediator,
                ILogger<RemoveVariantEndpoint> logger
            ) =>
            {
                var command = new RemoveVariantCommand(productId, variantId);

                return await HandleAsync(
                    mediator,
                    command,
                    _ => Results.Ok(SuccessResult.Success("Successfully removed product variant.")),
                    ex => logger.ProductFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Product.Delete")
        .WithTags("Products")
        .WithSummary("Remove a product variant")
        .WithDescription("Removes a variant from an existing product.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
