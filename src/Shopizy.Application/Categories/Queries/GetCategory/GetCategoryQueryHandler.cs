using ErrorOr;
using MediatR;
using shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace shopizy.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler<GetCategoryQuery, ErrorOr<Category?>>
{
    public async Task<ErrorOr<Category?>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategoryByIdAsync(CategoryId.Create(request.CategoryId));
    }
}
