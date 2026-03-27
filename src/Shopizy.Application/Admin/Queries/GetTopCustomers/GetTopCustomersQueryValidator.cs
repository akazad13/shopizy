using FluentValidation;

namespace Shopizy.Application.Admin.Queries.GetTopCustomers;

public class GetTopCustomersQueryValidator : AbstractValidator<GetTopCustomersQuery>
{
    public GetTopCustomersQueryValidator()
    {
        RuleFor(q => q.Count)
            .InclusiveBetween(1, 100)
            .WithMessage("Count must be between 1 and 100.");
    }
}
