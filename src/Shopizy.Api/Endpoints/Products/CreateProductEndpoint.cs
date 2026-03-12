using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Products;

/// <summary>
/// Endpoint for creating a new product.
/// </summary>
public class CreateProductEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/products", async (Guid userId, CreateProductRequest request, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<CreateProductEndpoint> logger) =>
        {
            var command = mapper.Map<CreateProductCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                product => Results.Ok(mapper.Map<ProductResponse>(product)),
                ex => logger.ProductCreationError(ex)
            );
        })
        .RequireAuthorization("SellerOrAdmin")
        .WithTags("Products")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Creates a new product";
            operation.Description = "This endpoint allows a seller or admin to create a new product in the system.";
            return operation;
        })
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
