using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

namespace Shopizy.Api.Endpoints.Products;

public class UpdateProductEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/products/{productId:guid}", async (Guid userId, Guid productId, UpdateProductRequest request, ISender mediator, IMapper mapper, ILogger<UpdateProductEndpoint> logger) =>
        {
            var command = mapper.Map<UpdateProductCommand>((userId, productId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated product.")),
                ex => logger.ProductUpdateError(ex)
            );
        })
        .RequireAuthorization("SellerOrAdmin")
        .WithTags("Products")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
