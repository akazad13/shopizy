using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

public class UpdateProductEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1.0/admin/products/{productId:guid}", async (Guid productId, UpdateProductRequest request, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<UpdateProductEndpoint> logger) =>
        {
            var command = mapper.Map<UpdateProductCommand>((currentUser.GetCurrentUserId(), productId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated product.")),
                ex => logger.ProductUpdateError(ex)
            );
        })
        .RequireAuthorization("Product.Modify")
        .WithTags("Products")
        .WithSummary("Update product")
        .WithDescription("Updates the details of an existing product.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
