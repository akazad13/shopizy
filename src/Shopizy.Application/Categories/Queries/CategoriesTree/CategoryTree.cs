namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public class CategoryTree
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public IReadOnlyList<CategoryTree>? Children { get; set; }
}
