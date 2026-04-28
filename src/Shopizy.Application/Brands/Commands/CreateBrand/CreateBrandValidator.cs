using FluentValidation;

namespace Shopizy.Application.Brands.Commands.CreateBrand;

public class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(brand => brand.Name).NotEmpty().MaximumLength(50);
        RuleFor(brand => brand.LogoUrl).MaximumLength(500).When(brand => brand.LogoUrl is not null);
        RuleFor(brand => brand.Country).NotEmpty().MaximumLength(100);
    }
}
