using FluentValidation;
using Shopizy.Application.Common.Validation;

namespace Shopizy.Application.GiftCards.Queries.GetGiftCards;

public class GetGiftCardsQueryValidator : AbstractValidator<GetGiftCardsQuery>
{
    public GetGiftCardsQueryValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();
    }
}
