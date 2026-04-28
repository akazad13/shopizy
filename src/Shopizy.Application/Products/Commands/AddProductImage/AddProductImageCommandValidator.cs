using FluentValidation;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.File).NotNull();
    }
}
