using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

public class DeleteProductEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/products/{productId:guid}", async (Guid userId, Guid productId, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<DeleteProductEndpoint> logger) =>
        {
            var command = mapper.Map<DeleteProductCommand>((userId, productId));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully deleted product.")),
                ex => logger.ProductDeleteError(ex)
            );
        })
        .RequireAuthorization("SellerOrAdmin")
        .WithTags("Products")
        .WithSummary("Delete product")
        .WithDescription("Deletes a specific product from the system.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
