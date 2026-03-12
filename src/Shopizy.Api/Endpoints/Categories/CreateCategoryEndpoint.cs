using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class CreateCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/categories", async (Guid userId, CreateCategoryRequest request, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<CreateCategoryEndpoint> logger) =>
        {
            var command = mapper.Map<CreateCategoryCommand>((userId, request));

            return await HandleAsync(
                mediator,
                command,
                category => Results.Ok(mapper.Map<CategoryResponse>(category)),
                ex => logger.CategoryCreationError(ex)
            );
        })
        .RequireAuthorization("AdminOnly")
        .WithTags("Categories")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create category";
            operation.Description = "Creates a new product category.";
            return operation;
        })
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
