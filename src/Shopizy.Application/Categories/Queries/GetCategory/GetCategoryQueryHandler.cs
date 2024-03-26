using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler<GetCategoryQuery, ErrorOr<Category?>>
{
    public async Task<ErrorOr<Category?>> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId));
    }
}
