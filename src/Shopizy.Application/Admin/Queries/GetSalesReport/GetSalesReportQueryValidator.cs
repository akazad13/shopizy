using FluentValidation;

namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public class GetSalesReportQueryValidator : AbstractValidator<GetSalesReportQuery>
{
    private const int MaxDateRangeDays = 90;

    public GetSalesReportQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date.")
            .Must((query, endDate) => (endDate - query.StartDate).TotalDays <= MaxDateRangeDays)
            .WithMessage($"Date range cannot exceed {MaxDateRangeDays} days.");
    }
}
