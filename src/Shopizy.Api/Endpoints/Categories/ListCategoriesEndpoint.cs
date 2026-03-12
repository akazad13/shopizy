using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.ListCategories;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class ListCategoriesEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories", async ([FromServices] IDispatcher mediator, IMapper mapper, ILogger<ListCategoriesEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new ListCategoriesQuery(),
                categories => Results.Ok(mapper.Map<IReadOnlyList<CategoryResponse>>(categories)),
                ex => logger.CategoryFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Categories")
        .WithOpenApi(operation =>
        {
            operation.Summary = "List categories";
            operation.Description = "Retrieves a flat list of all categories.";
            return operation;
        })
        .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
