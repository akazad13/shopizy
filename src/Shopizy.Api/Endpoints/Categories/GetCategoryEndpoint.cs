using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;

namespace Shopizy.Api.Endpoints.Categories;

public class GetCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1.0/categories/{categoryId:guid}", async (Guid categoryId, ISender mediator, IMapper mapper, ILogger<GetCategoryEndpoint> logger) =>
        {
            try
            {
                var query = mapper.Map<GetCategoryQuery>(categoryId);
                var result = await mediator.Send(query);

                return result.Match(
                    category => Results.Ok(mapper.Map<CategoryResponse>(category)),
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
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
