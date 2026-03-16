using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand, ErrorOr<Success>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<Success>> Handle(
        DeleteCategoryCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(
            CategoryId.Create(cmd.CategoryId)
        );
        if (category is null)
        {
            return CustomErrors.Category.CategoryNotFound;
        }

        _categoryRepository.Remove(category);

        return Result.Success;
    }
}
