namespace Shopizy.Contracts.Category;

/// <summary>
/// Represents a category response.
/// </summary>
/// <param name="Id">The unique identifier of the category.</param>
/// <param name="Name">The name of the category.</param>
/// <param name="ParentId">The identifier of the parent category, if any.</param>
public record CategoryResponse(Guid Id, string Name, Guid? ParentId);
