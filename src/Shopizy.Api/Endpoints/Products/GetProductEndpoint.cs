using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

public class GetProductEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/products/{productId:guid}", async (Guid productId, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetProductEndpoint> logger) =>
        {
            var query = mapper.Map<GetProductQuery>(productId);

            return await HandleAsync(
                mediator,
                query,
                product => Results.Ok(mapper.Map<ProductDetailResponse>(product)),
                ex => logger.ProductFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Products")
        .Produces<ProductDetailResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
