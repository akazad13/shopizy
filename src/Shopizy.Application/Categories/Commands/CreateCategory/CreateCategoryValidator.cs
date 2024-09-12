using FluentValidation;

namespace Shopizy.Application.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        _ = RuleFor(category => category.Name).NotEmpty().MaximumLength(100);
    }
}
