using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IQueryHandler<ListCategoriesQuery, ErrorOr<List<CategoryItem>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<List<CategoryItem>>> Handle(
        ListCategoriesQuery query,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryRepository.GetCategoriesAsync();
        return categories.Select(c => new CategoryItem(c.Id.Value, c.Name, c.ParentId)).ToList();
    }
}
