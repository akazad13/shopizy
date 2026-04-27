using FluentValidation;
using Shopizy.Application.Common.Validation;

namespace Shopizy.Application.ProductReviews.Queries.GetProductReviews;

public class GetProductReviewsQueryValidator : AbstractValidator<GetProductReviewsQuery>
{
    public GetProductReviewsQueryValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();
    }
}
