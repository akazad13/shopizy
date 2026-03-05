using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Queries.GetProducts;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

namespace Shopizy.Api.Endpoints.Products;

public class SearchProductsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/products", async ([AsParameters] ProductsCriteria criteria, ISender mediator, IMapper mapper, ILogger<SearchProductsEndpoint> logger) =>
        {
            var query = mapper.Map<GetProductsQuery>(criteria);

            return await HandleAsync(
                mediator,
                query,
                products => Results.Ok(mapper.Map<IReadOnlyList<ProductResponse>>(products)),
                ex => logger.ProductFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Products")
        .Produces<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
