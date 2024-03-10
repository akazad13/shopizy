using ErrorOr;
using MediatR;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;

namespace shopizy.Application.Categories.Queries.ListCategoriesQuery;

public class ListCategoriesQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler< ListCategoriesQuery, ErrorOr<List<Category>>>
{
    public async Task<ErrorOr<List<Category>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategories();
    }
}