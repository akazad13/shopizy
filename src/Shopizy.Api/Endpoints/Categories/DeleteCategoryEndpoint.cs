using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/categories/{categoryId:guid}", async (Guid userId, Guid categoryId, ISender mediator, IMapper mapper, ILogger<DeleteCategoryEndpoint> logger) =>
        {
            try
            {
                var command = mapper.Map<DeleteCategoryCommand>((userId, categoryId));
                var result = await mediator.Send(command);

                return result.Match(
                    success => Results.Ok(SuccessResult.Success("Successfully deleted category.")),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CategoryDeleteError(ex);
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
