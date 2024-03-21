using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Categories.Queries.TestUtils;

public static class GetCategoryQueryUtils
{
    public static GetCategoryQuery CreateQuery()
    {
        return new GetCategoryQuery(Constants.Category.Id);
    }
}
