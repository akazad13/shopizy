using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        : IRequestHandler<UpdateCategoryCommand, ErrorOr<Category>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<Category>> Handle(
        UpdateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(CategoryId.Create(cmd.CategoryId));
        if (category is null)
        {
            return CustomErrors.Category.CategoryNotFound;
        }

        category.Update(cmd.Name, cmd.ParentId);

        _categoryRepository.Update(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Category.CategoryNotUpdated;
        }

        return category;
    }
}
