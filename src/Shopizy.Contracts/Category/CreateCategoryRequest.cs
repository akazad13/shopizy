namespace shopizy.Contracts.Category;

public record CreateCategoryRequest(string Name, Guid? ParentId);
