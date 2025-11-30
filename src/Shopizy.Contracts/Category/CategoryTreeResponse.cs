namespace shopizy.Contracts.Category;

/// <summary>
/// Represents a hierarchical category tree response.
/// </summary>
/// <param name="Id">The unique identifier of the category.</param>
/// <param name="Name">The name of the category.</param>
/// <param name="ParentId">The identifier of the parent category, if any.</param>
/// <param name="Children">The list of child categories.</param>
public record CategoryTreeResponse(
    Guid Id,
    string Name,
    Guid? ParentId,
    List<CategoryTreeResponse>? Children
);
