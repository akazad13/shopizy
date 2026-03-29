using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class UpdateCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/v1.0/admin/categories/{categoryId:guid}", async (Guid categoryId, [FromBody] UpdateCategoryRequest request, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, ILogger<UpdateCategoryEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new UpdateCategoryCommand(currentUser.GetCurrentUserId(), categoryId, request.Name, request.ParentId),
                success => Results.Ok(SuccessResult.Success("Successfully updated category.")),
                ex => logger.CategoryUpdateError(ex)
            );
        })
        .RequireAuthorization("Category.Modify")
        .WithTags("Categories")
        .WithSummary("Update category")
        .WithDescription("Updates the details of an existing category.")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
