namespace shopizy.Contracts.Category;

public record CategoryResponse(Guid Id, string Name, Guid? ParentId);
