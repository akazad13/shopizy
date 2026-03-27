using FluentValidation;

namespace Shopizy.Application.Admin.Queries.GetTopProducts;

public class GetTopProductsQueryValidator : AbstractValidator<GetTopProductsQuery>
{
    public GetTopProductsQueryValidator()
    {
        RuleFor(q => q.Count)
            .InclusiveBetween(1, 100)
            .WithMessage("Count must be between 1 and 100.");
    }
}
