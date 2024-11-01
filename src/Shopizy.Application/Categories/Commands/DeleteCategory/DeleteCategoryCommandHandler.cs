using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteCategoryCommand, IResult<GenericResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<GenericResponse>> Handle(
        DeleteCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(
            CategoryId.Create(cmd.CategoryId)
        );
        if (category is null)
        {
            return Response<GenericResponse>.ErrorResponse(
                [CustomErrors.Category.CategoryNotFound]
            );
        }

        _categoryRepository.Remove(category);

        if (await _categoryRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse(
                [CustomErrors.Category.CategoryNotDeleted]
            );
        }

        return Response<GenericResponse>.SuccessResponese("Successfully deleted category.");
    }
}
