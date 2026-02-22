using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Products;

public class DeleteProductImageEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/products/{productId:guid}/image/{imageId:guid}", async (Guid userId, Guid productId, Guid imageId, ISender mediator, IMapper mapper, ILogger<DeleteProductImageEndpoint> logger) =>
        {
            var command = mapper.Map<DeleteProductImageCommand>((userId, productId, imageId));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully deleted product image.")),
                ex => logger.ProductImageDeleteError(ex)
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
