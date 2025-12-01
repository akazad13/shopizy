namespace Shopizy.Application.Categories.Queries.CategoriesTree;

public class CategoryTree
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public List<CategoryTree>? Children { get; set; }
}
