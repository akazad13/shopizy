using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/categories", async (Guid userId, CreateCategoryRequest request, ISender mediator, IMapper mapper, ILogger<CreateCategoryEndpoint> logger) =>
        {
            try
            {
                var command = mapper.Map<CreateCategoryCommand>((userId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    category => Results.Ok(mapper.Map<CategoryResponse>(category)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CategoryCreationError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .RequireAuthorization("AdminOnly")
        .WithTags("Categories")
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
