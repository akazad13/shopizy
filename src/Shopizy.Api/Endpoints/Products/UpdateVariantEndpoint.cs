using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.UpdateVariant;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

/// <summary>
/// Endpoint for updating a product variant.
/// </summary>
public class UpdateVariantEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "api/v1.0/admin/products/{productId:guid}/variants/{variantId:guid}",
            async (
                Guid productId,
                Guid variantId,
                UpdateVariantRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<UpdateVariantEndpoint> logger
            ) =>
            {
                var command = mapper.Map<UpdateVariantCommand>((productId, variantId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    variant => Results.Ok(mapper.Map<ProductVariantResponse>(variant)),
                    ex => logger.ProductFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Product.Modify")
        .WithTags("Products")
        .WithSummary("Update a product variant")
        .WithDescription("Updates an existing product variant's details.")
        .Produces<ProductVariantResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
