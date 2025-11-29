using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.TestUtils;

public static class GetCategoryQueryUtils
{
    public static GetCategoryQuery CreateQuery(CategoryId categoryId)
    {
        return new GetCategoryQuery(categoryId.Value);
    }
}
