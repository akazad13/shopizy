namespace shopizy.Contracts.Product;

/// <summary>
/// Represents criteria for searching and filtering products.
/// </summary>
/// <param name="Name">Filter by product name.</param>
/// <param name="CategoryIds">Filter by category identifiers.</param>
/// <param name="AverageRating">Filter by minimum average rating.</param>
/// <param name="PageNumber">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public record ProductsCriteria(
    string? Name,
    IList<Guid>? CategoryIds,
    decimal? AverageRating,
    int PageNumber = 1,
    int PageSize = 10
);
