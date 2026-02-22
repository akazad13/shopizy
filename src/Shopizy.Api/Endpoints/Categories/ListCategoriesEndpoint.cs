using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.ListCategories;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class ListCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories", async (ISender mediator, IMapper mapper, ILogger<ListCategoriesEndpoint> logger) =>
        {
            try
            {
                var result = await mediator.Send(new ListCategoriesQuery());

                return result.Match(
                    categories => Results.Ok(mapper.Map<List<CategoryResponse>>(categories)),
                    CustomResults.Problem
                );
            }
            catch (Exception ex)
            {
                logger.CategoryFetchError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
            }
        })
        .AllowAnonymous()
        .WithTags("Categories")
        .Produces<List<CategoryResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
