using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Queries.GetProducts;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

namespace Shopizy.Api.Endpoints.Products;

public class SearchProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/products", async ([AsParameters] ProductsCriteria criteria, ISender mediator, IMapper mapper, ILogger<SearchProductsEndpoint> logger) =>
        {
            try
            {
                var query = mapper.Map<GetProductsQuery>(criteria);
                var result = await mediator.Send(query);

                return result.Match(
                    products => Results.Ok(mapper.Map<List<ProductResponse>>(products)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.ProductFetchError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .AllowAnonymous()
        .WithTags("Products")
        .Produces<List<ProductResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
