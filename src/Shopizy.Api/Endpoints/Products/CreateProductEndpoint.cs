using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;

namespace Shopizy.Api.Endpoints.Products;

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/products", async (Guid userId, CreateProductRequest request, ISender mediator, IMapper mapper, ILogger<CreateProductEndpoint> logger) =>
        {
            try
            {
                var command = mapper.Map<CreateProductCommand>((userId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    product => Results.Ok(mapper.Map<ProductResponse>(product)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.ProductCreationError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .RequireAuthorization("SellerOrAdmin")
        .WithTags("Products")
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
