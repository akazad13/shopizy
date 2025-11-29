using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Category
    {
        public static readonly CategoryId Id = CategoryId.Create(Guid.NewGuid());

        public const string Name = "Category Name";
        public static readonly Guid ParentId = Guid.NewGuid();
    }
}
