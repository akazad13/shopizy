namespace shopizy.Contracts.Product;

public record ProductsCriteriaRequest(
    string? Name,
    IList<Guid>? CategoryIds,
    double? AverageRating,
    int PageNumber = 1,
    int PageSize = 10
);
