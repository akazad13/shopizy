using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Categories;

public sealed class Category : AggregateRoot<CategoryId, Guid>
{
    public string Name { get; set; }
    public CategoryId ParentCategoryId { get; set; }

    public static Category Create(string name, CategoryId categoryId)
    {
        return new Category(CategoryId.CreateUnique(), name, categoryId);
    }

    private Category(CategoryId categoryId, string name, CategoryId parentCategoryId)
        : base(categoryId)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Category() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
