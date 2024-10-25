using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateCategoryCommand, IResult<Category>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<Category>> Handle(
        UpdateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(
            CategoryId.Create(cmd.CategoryId)
        );
        if (category is null)
        {
            return Response<Category>.ErrorResponse([CustomErrors.Category.CategoryNotFound]);
        }

        category.Update(cmd.Name, cmd.ParentId);

        _categoryRepository.Update(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
        {
            return Response<Category>.ErrorResponse([CustomErrors.Category.CategoryNotUpdated]);
        }
        return Response<Category>.SuccessResponese(category);
    }
}
