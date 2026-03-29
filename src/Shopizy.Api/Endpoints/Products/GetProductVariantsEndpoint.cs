using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Queries.GetProductVariants;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

/// <summary>
/// Endpoint for retrieving all variants of a product.
/// </summary>
public class GetProductVariantsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/products/{productId:guid}/variants",
            async (
                Guid productId,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetProductVariantsEndpoint> logger
            ) =>
            {
                var query = new GetProductVariantsQuery(productId);

                return await HandleAsync(
                    mediator,
                    query,
                    variants => Results.Ok(mapper.Map<IReadOnlyList<ProductVariantResponse>>(variants)),
                    ex => logger.ProductFetchError(ex)
                );
            }
        )
        .AllowAnonymous()
        .WithTags("Products")
        .WithSummary("Get product variants")
        .WithDescription("Retrieves all variants for a specific product.")
        .Produces<IReadOnlyList<ProductVariantResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
