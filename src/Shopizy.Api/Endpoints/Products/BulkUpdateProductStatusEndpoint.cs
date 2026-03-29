using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.BulkUpdateProductStatus;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Products;

public class BulkUpdateProductStatusEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/admin/products/bulk-update-status", async (
            [FromBody] BulkUpdateProductStatusRequest request,
            [FromServices] IDispatcher mediator,
            ILogger<BulkUpdateProductStatusEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new BulkUpdateProductStatusCommand(request.ProductIds, request.IsActive),
                _ => Results.Ok(SuccessResult.Success("Successfully updated product statuses.")),
                ex => logger.ProductUpdateError(ex)
            );
        })
        .RequireAuthorization("Product.Modify")
        .WithTags("Products")
        .WithSummary("Bulk update product status")
        .WithDescription("Updates the active status for multiple products.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
