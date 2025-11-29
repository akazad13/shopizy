using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

/// <summary>
/// Handles the <see cref="CreateCategoryCommand"/> to create new categories.
/// </summary>
public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, ErrorOr<Category>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    /// <summary>
    /// Handles creating a new category with duplicate name validation.
    /// </summary>
    /// <param name="cmd">The create category command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category or an error.</returns>
    public async Task<ErrorOr<Category>> Handle(
        CreateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        if (await _categoryRepository.GetCategoryByNameAsync(cmd.Name))
        {
            return CustomErrors.Category.DuplicateName;
        }

        var category = Category.Create(cmd.Name, cmd.ParentId);
        await _categoryRepository.AddAsync(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Category.CategoryNotCreated;
        }
        return category;
    }
}
