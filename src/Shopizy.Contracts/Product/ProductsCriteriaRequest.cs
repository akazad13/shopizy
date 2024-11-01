namespace shopizy.Contracts.Product;

public record ProductsCriteriaRequest(
    string? Name,
    IList<Guid>? CategoryIds,
    double? AverageRating
);
