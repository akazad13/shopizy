namespace shopizy.Contracts.Product;

public record ProductsCriteria(
    string? Name,
    IList<Guid>? CategoryIds,
    decimal? AverageRating,
    int PageNumber = 1,
    int PageSize = 10
);
