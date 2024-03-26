using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler<ListCategoriesQuery, ErrorOr<List<Category>>>
{
    public async Task<ErrorOr<List<Category>>> Handle(ListCategoriesQuery query, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategoriesAsync();
    }
}