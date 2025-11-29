using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.UnitTests.Categories.TestUtils;

public static class CategoryFactory
{
    public static Category Create()
    {
        return Category.Create(Constants.Category.Name, Constants.Category.ParentId);
    }
}
