using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/categories/{categoryId:guid}", async (Guid userId, Guid categoryId, UpdateCategoryRequest request, ISender mediator, IMapper mapper, ILogger<UpdateCategoryEndpoint> logger) =>
        {
            try
            {
                var command = mapper.Map<UpdateCategoryCommand>((userId, categoryId, request));
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(SuccessResult.Success("Successfully updated category.")),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CategoryUpdateError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .RequireAuthorization("AdminOnly")
        .WithTags("Categories")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
