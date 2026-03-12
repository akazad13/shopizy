using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class GetCategoryTreeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories/tree", async ([FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetCategoryTreeEndpoint> logger) =>
        {
            return await HandleAsync(
                mediator,
                new CategoriesTreeQuery(),
                categories => Results.Ok(mapper.Map<List<CategoryTreeResponse>>(categories)),
                ex => logger.CategoryFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Categories")
        .WithSummary("Get category tree")
        .WithDescription("Retrieves the hierarchical structure of all categories.")
        .Produces<List<CategoryTreeResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
