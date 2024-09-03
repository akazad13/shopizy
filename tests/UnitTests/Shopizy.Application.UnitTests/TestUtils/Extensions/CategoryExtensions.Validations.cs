using FluentAssertions;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CategoryExtensions
{
    public static void ValidateResult(this Category category, CreateCategoryCommand command)
    {
        _ = category.Id.Should().BeOfType(typeof(CategoryId));
        _ = category.Name.Should().Be(command.Name);
        _ = category.ParentId.Should().Be(command.ParentId);
    }

    public static void ValidateResult(this Category category, GetCategoryQuery query)
    {
        _ = category.Id.Should().BeOfType(typeof(CategoryId));
        _ = category.Name.Should().BeOfType(typeof(string));
    }

    public static void ValidateResult(this Category category, UpdateCategoryCommand command)
    {
        _ = category.Id.Should().BeOfType(typeof(CategoryId));
        _ = category.Name.Should().Be(command.Name);
        _ = category.ParentId.Should().Be(command.ParentId);
    }
}
