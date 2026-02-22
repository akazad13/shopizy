using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Products;

public class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/products/{productId:guid}", async (Guid userId, Guid productId, ISender mediator, IMapper mapper, ILogger<DeleteProductEndpoint> logger) =>
        {
            try
            {
                var command = mapper.Map<DeleteProductCommand>((userId, productId));
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(SuccessResult.Success("Successfully deleted product.")),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.ProductDeleteError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
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
