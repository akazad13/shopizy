using FluentValidation;

namespace Shopizy.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(category => category.Name).NotEmpty().MaximumLength(100);
    }
}
