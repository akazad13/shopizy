using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class DeleteCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v1.0/admin/categories/{categoryId:guid}", async (Guid categoryId, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, ILogger<DeleteCategoryEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new DeleteCategoryCommand(currentUser.GetCurrentUserId(), categoryId),
                success => Results.Ok(SuccessResult.Success("Successfully deleted category.")),
                ex => logger.CategoryDeleteError(ex)
            );
        })
        .RequireAuthorization("Category.Delete")
        .WithTags("Categories")
        .WithSummary("Delete category")
        .WithDescription("Deletes an existing product category.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
