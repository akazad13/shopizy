using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler<GetCategoryQuery, ErrorOr<Category?>>
{
    public async Task<ErrorOr<Category?>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
    }
}
