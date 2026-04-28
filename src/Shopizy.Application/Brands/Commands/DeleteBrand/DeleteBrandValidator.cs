using FluentValidation;

namespace Shopizy.Application.Brands.Commands.DeleteBrand;

public class DeleteBrandValidator : AbstractValidator<DeleteBrandCommand>
{
    public DeleteBrandValidator()
    {
        RuleFor(brand => brand.BrandId).NotEmpty();
    }
}
