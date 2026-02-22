using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class GetCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories/{categoryId:guid}", async (Guid categoryId, ISender mediator, IMapper mapper, ILogger<GetCategoryEndpoint> logger) =>
        {
            var query = mapper.Map<GetCategoryQuery>(categoryId);

            return await HandleAsync(
                mediator,
                query,
                category => Results.Ok(mapper.Map<CategoryResponse>(category)),
                ex => logger.CategoryFetchError(ex)
            );
        })
        .AllowAnonymous()
        .WithTags("Categories")
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
