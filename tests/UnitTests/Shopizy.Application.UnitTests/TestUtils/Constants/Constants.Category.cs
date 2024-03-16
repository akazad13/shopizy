using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Category
    {
        public static readonly CategoryId CategoryId = CategoryId.Create(
            new Guid("15ad4ba5-3e8d-4278-bc68-582246d836f4")
        );
    }
}
