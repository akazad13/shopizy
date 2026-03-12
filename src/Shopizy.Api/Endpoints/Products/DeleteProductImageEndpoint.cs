using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

public class DeleteProductImageEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}", async (Guid userId, Guid productId, Guid imageId, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<DeleteProductImageEndpoint> logger) =>
        {
            var command = mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully deleted product image.")),
                ex => logger.ProductImageDeleteError(ex)
            );
        })
        .RequireAuthorization("Product.Modify")
        .WithTags("Products")
        .WithSummary("Delete product image")
        .WithDescription("Deletes a specific image from a product.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
