using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateCategoryCommand, ErrorOr<Success>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(
            CategoryId.Create(cmd.CategoryId)
        );
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
        return Result.Success;
    }
}
