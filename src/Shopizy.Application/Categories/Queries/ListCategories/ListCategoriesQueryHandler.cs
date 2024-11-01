using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<ListCategoriesQuery, IResult<List<Category>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<List<Category>>> Handle(
        ListCategoriesQuery query,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryRepository.GetCategoriesAsync();
        return Response<List<Category>>.SuccessResponese(categories);
    }
}
