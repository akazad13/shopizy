using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Errors;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository _categoryRepository)
        : IRequestHandler<UpdateCategoryCommand, ErrorOr<Category>>
{
    public async Task<ErrorOr<Category>> Handle(
        UpdateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId.Create(cmd.CategoryId));
        if (category is null)
            return Errors.Category.CategoryNotFound;

        category.Name = cmd.Name;
        category.ParentId = cmd.ParentId;

        _categoryRepository.Update(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
            return Errors.Category.CategoryNotUpdated;

        return category;
    }
}
