using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CategoryExtensions
{
    public static void ValidateResult(this Category category, CreateCategoryCommand command)
    {
        Assert.IsType<CategoryId>(category.Id);
        Assert.Equal(command.Name, category.Name);
        Assert.Equal(command.ParentId, category.ParentId);
    }

    public static void ValidateResult(this Category category, GetCategoryQuery query)
    {
        Assert.IsType<CategoryId>(category.Id);
        Assert.IsType<string>(category.Name);
    }
}
