using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products;

namespace Shopizy.Domain.Categories;

public class Category : AggregateRoot<CategoryId, Guid>
{
    private readonly List<Product> _products = [];
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public static Category Create(string name, Guid? parentId)
    {
        return new Category(CategoryId.CreateUnique(), name, parentId);
    }

    public void Update(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
    }

    private Category(CategoryId categoryId, string name, Guid? parentId) : base(categoryId)
    {
        Name = name;
        ParentId = parentId;
    }

    private Category() { }
}
