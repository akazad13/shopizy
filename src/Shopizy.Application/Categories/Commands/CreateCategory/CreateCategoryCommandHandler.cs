using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, IResult<Category>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<Category>> Handle(
        CreateCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        if (await _categoryRepository.GetCategoryByNameAsync(cmd.Name))
        {
            return Response<Category>.ErrorResponse([CustomErrors.Category.DuplicateName]);
        }

        var category = Category.Create(cmd.Name, cmd.ParentId);
        await _categoryRepository.AddAsync(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
        {
            return Response<Category>.ErrorResponse([CustomErrors.Category.CategoryNotCreated]);
        }
        return Response<Category>.SuccessResponese(category);
    }
}
