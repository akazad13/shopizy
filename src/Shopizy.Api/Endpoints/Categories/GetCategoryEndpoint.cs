using MapsterMapper;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

using Microsoft.AspNetCore.Mvc;
namespace Shopizy.Api.Endpoints.Categories;

public class GetCategoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories/{categoryId:guid}", async (Guid categoryId, [FromServices] IDispatcher mediator, IMapper mapper, ILogger<GetCategoryEndpoint> logger) =>
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
        .WithSummary("Get category by ID")
        .WithDescription("Retrieves a specific category's details.")
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
