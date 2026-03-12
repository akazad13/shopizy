using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class DeleteCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/users/{userId:guid}/categories/{categoryId:guid}", async (Guid userId, Guid categoryId, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<DeleteCategoryEndpoint> logger) =>
        {
            var command = mapper.Map<DeleteCategoryCommand>((userId, categoryId));

            return await HandleAsync(
                mediator,
                command,
                success => Results.Ok(SuccessResult.Success("Successfully deleted category.")),
                ex => logger.CategoryDeleteError(ex)
            );
        })
        .RequireAuthorization("AdminOnly")
        .WithTags("Categories")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Delete category";
            operation.Description = "Deletes an existing product category.";
            return operation;
        })
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
