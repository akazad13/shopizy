using FluentValidation;

namespace Shopizy.Application.Common.Validation;

public static class PaginationRules
{
    public const int MaxPageSize = 100;
    public const int MinPageNumber = 1;

    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThan(0)
            .WithMessage($"PageSize must be greater than 0.")
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"PageSize must be {MaxPageSize} or less.");

    public static IRuleBuilderOptions<T, int> ValidPageNumber<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThanOrEqualTo(MinPageNumber)
            .WithMessage($"PageNumber must be {MinPageNumber} or greater.");
}
