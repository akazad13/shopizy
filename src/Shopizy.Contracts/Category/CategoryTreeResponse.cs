namespace shopizy.Contracts.Category;

public record CategoryTreeResponse(
    Guid Id,
    string Name,
    Guid? ParentId,
    List<CategoryTreeResponse>? Children
);
