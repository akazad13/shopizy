using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

public class AddProductImageEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/products/{productId:guid}/image", async (Guid userId, Guid productId, IFormFile file, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<AddProductImageEndpoint> logger) =>
        {
            var command = new AddProductImageCommand(userId, productId, file);

            return await HandleAsync(
                mediator,
                command,
                productImage => Results.Ok(mapper.Map<ProductImageResponse>(productImage)),
                ex => logger.ProductImageAdditionError(ex)
            );
        })
        .RequireAuthorization("SellerOrAdmin")
        .WithTags("Products")
        .WithSummary("Add product image")
        .WithDescription("Uploads a new image for a specific product.")
        .DisableAntiforgery()
        .Produces<ProductImageResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
