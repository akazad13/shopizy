using FluentValidation;
using Shopizy.Application.Common.Validation;

namespace Shopizy.Application.PromoCodes.Queries.GetPromoCodes;

public class GetPromoCodesQueryValidator : AbstractValidator<GetPromoCodesQuery>
{
    public GetPromoCodesQueryValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();
    }
}
