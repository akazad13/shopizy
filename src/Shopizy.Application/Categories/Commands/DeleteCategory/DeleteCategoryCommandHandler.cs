using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(ICategoryRepository _categoryRepository)
        : IRequestHandler<DeleteCategoryCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId.Create(cmd.CategoryId));
        if (category is null)
            return CustomErrors.Category.CategoryNotFound;

        _categoryRepository.Remove(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Category.CategoryNotDeleted;

        return Result.Success;
    }
}
