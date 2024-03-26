using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.Errors;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository _categoryRepository)
        : IRequestHandler<CreateCategoryCommand, ErrorOr<Category>>
{
    public async Task<ErrorOr<Category>> Handle(
        CreateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        if (await _categoryRepository.GetCategoryByNameAsync(cmd.Name))
            return Errors.Category.DuplicateName;

        var category = Category.Create(cmd.Name, cmd.ParentId);
        await _categoryRepository.AddAsync(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
            return Errors.Category.CategoryNotCreated;

        return category;
    }
}
