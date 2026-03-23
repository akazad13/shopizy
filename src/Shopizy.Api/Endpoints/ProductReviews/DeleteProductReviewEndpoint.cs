using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.ProductReviews.Commands.DeleteProductReview;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.ProductReviews;

public class DeleteProductReviewEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "api/v1.0/admin/products/{productId:guid}/reviews/{reviewId:guid}",
            async (
                Guid productId,
                Guid reviewId,
                [FromServices] IDispatcher mediator,
                ILogger<DeleteProductReviewEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new DeleteProductReviewCommand(productId, reviewId),
                    _ => Results.Ok(SuccessResult.Success("Review deleted successfully.")),
                    ex => logger.ProductReviewDeleteError(ex)
                );
            }
        )
        .RequireAuthorization("Admin.DeleteProductReview")
        .WithTags("ProductReviews")
        .WithSummary("Delete a product review (admin)")
        .WithDescription("Allows an admin to moderate and remove a product review.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
