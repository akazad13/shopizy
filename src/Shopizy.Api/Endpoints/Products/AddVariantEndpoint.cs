using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.AddVariant;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

/// <summary>
/// Endpoint for adding a variant to a product.
/// </summary>
public class AddVariantEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/admin/products/{productId:guid}/variants",
            async (
                Guid productId,
                AddVariantRequest request,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<AddVariantEndpoint> logger
            ) =>
            {
                var command = mapper.Map<AddVariantCommand>((productId, request));

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
        .WithSummary("Add a variant to a product")
        .WithDescription("Adds a new variant (e.g. size/color combination) to an existing product.")
        .Produces<ProductVariantResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
