using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class GetCategoryTreeEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories/tree", async (ISender mediator, IMapper mapper, ILogger<GetCategoryTreeEndpoint> logger) =>
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
        .Produces<List<CategoryTreeResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
