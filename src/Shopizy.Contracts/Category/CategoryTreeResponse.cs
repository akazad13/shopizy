namespace shopizy.Contracts.Category;

public class CategoryTreeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public IEnumerable<CategoryTreeResponse>? Children { get; set; }
}
