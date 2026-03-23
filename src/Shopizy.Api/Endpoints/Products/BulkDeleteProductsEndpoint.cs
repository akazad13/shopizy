using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.BulkDeleteProducts;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

public class BulkDeleteProductsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/admin/products/bulk-delete", async (
            [FromBody] BulkDeleteProductsRequest request,
            [FromServices] IDispatcher mediator,
            ILogger<BulkDeleteProductsEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new BulkDeleteProductsCommand(request.ProductIds),
                _ => Results.Ok(SuccessResult.Success("Successfully deleted products.")),
                ex => logger.ProductDeleteError(ex)
            );
        })
        .RequireAuthorization("Product.Delete")
        .WithTags("Products")
        .WithSummary("Bulk delete products")
        .WithDescription("Deletes multiple products from the system.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status404NotFound)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
