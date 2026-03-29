using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.ProductReviews.Queries.GetProductReviews;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.ProductReview;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.ProductReviews;

public class GetProductReviewsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/products/{productId:guid}/reviews",
            async (
                Guid productId,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetProductReviewsEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new GetProductReviewsQuery(productId, pageNumber, pageSize),
                    reviews => Results.Ok(mapper.Map<List<ProductReviewResponse>>(reviews)),
                    ex => logger.ProductReviewFetchError(ex)
                );
            }
        )
        .AllowAnonymous()
        .WithTags("ProductReviews")
        .WithSummary("Get product reviews")
        .WithDescription("Returns a paginated list of reviews for a specific product.")
        .Produces<List<ProductReviewResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
