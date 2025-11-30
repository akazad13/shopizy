namespace Shopizy.Contracts.Category;

/// <summary>
/// Represents a request to update an existing category.
/// </summary>
/// <param name="Name">The new name of the category.</param>
/// <param name="ParentId">The new parent category identifier.</param>
public record UpdateCategoryRequest(string Name, Guid? ParentId);
