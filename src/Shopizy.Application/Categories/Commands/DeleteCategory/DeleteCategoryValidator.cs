using Shopizy.SharedKernel.Application.Messaging;
using FluentValidation;

namespace Shopizy.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator() { }
}

