using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class CreateCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/admin/categories", async ([FromBody] CreateCategoryRequest request, [FromServices] ICurrentUser currentUser, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<CreateCategoryEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new CreateCategoryCommand(currentUser.GetCurrentUserId(), request.Name, request.ParentId),
                category => Results.Ok(mapper.Map<CategoryResponse>(category)),
                ex => logger.CategoryCreationError(ex)
            );
        })
        .RequireAuthorization("Category.Create")
        .WithTags("Categories")
        .WithSummary("Create category")
        .WithDescription("Creates a new product category.")
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
