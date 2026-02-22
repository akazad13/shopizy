using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class UpdateCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/users/{userId:guid}/categories/{categoryId:guid}", async (Guid userId, Guid categoryId, UpdateCategoryRequest request, ISender mediator, IMapper mapper, ILogger<UpdateCategoryEndpoint> logger) =>
        {
            var command = mapper.Map<UpdateCategoryCommand>((userId, categoryId, request));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully updated category.")),
                ex => logger.CategoryUpdateError(ex)
            );
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
