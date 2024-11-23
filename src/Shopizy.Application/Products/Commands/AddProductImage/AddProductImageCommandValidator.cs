using FluentValidation;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(category => category.File).NotNull();
    }
}
