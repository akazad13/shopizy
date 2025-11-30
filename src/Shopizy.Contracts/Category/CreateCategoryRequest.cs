namespace Shopizy.Contracts.Category;

/// <summary>
/// Represents a request to create a new category.
/// </summary>
/// <param name="Name">The name of the category.</param>
/// <param name="ParentId">The optional parent category identifier.</param>
public record CreateCategoryRequest(string Name, Guid? ParentId);
