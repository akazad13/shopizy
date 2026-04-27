using FluentValidation;
using Shopizy.Application.Common.Validation;

namespace Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;

public class GetProductQuestionsQueryValidator : AbstractValidator<GetProductQuestionsQuery>
{
    public GetProductQuestionsQueryValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();
    }
}
