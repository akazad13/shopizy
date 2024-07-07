using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<ListCategoriesQuery, ErrorOr<List<Category>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<List<Category>>> Handle(ListCategoriesQuery query, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategoriesAsync();
    }
}