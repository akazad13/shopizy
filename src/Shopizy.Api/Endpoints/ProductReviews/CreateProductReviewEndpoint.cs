using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.ProductReviews.Commands.CreateProductReview;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.ProductReview;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.ProductReviews;

public class CreateProductReviewEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/v1.0/products/{productId:guid}/reviews",
            async (
                Guid productId,
                CreateProductReviewRequest request,
                [FromServices] ICurrentUser currentUser,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<CreateProductReviewEndpoint> logger
            ) =>
            {
                var userId = currentUser.GetCurrentUserId();
                var command = mapper.Map<CreateProductReviewCommand>((userId, productId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    review => Results.Ok(mapper.Map<ProductReviewResponse>(review)),
                    ex => logger.ProductReviewCreationError(ex)
                );
            }
        )
        .RequireAuthorization("ProductReview.Create")
        .WithTags("ProductReviews")
        .WithSummary("Submit a product review")
        .WithDescription("Allows an authenticated user to submit a rating and comment for a product.")
        .Produces<ProductReviewResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
