using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryQuery, IResult<Category?>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<Category?>> Handle(
        GetCategoryQuery query,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(
            CategoryId.Create(query.CategoryId)
        );
        return Response<Category?>.SuccessResponese(category);
    }
}
