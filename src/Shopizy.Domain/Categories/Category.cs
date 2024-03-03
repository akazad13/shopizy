using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products;

namespace Shopizy.Domain.Categories;

public class Category : AggregateRoot<CategoryId, Guid>
{
    private readonly List<Product> _products = [];
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public static Category Create(string name, Guid? parentId)
    {
        return new Category(CategoryId.CreateUnique(), name, parentId);
    }

    private Category(CategoryId categoryId, string name, Guid? parentId) : base(categoryId)
    {
        Name = name;
        ParentId = parentId;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Category() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
