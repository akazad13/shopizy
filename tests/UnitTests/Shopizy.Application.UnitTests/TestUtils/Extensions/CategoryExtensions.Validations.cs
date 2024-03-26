using FluentAssertions;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CategoryExtensions
{
    public static void ValidateCreatedForm(this Category category, CreateCategoryCommand command)
    {
        category.Id.Should().BeOfType(typeof(CategoryId));
        category.Name.Should().Be(command.Name);
        category.ParentId.Should().Be(command.ParentId);
    }

    public static void ValidateCreatedForm(this Category category, GetCategoryQuery query)
    {
        category.Id.Should().BeOfType(typeof(CategoryId));
        category.Name.Should().BeOfType(typeof(string));
    }

    public static void ValidateCreatedForm(this Category category, UpdateCategoryCommand command)
    {
        category.Id.Should().BeOfType(typeof(CategoryId));
        category.Name.Should().Be(command.Name);
        category.ParentId.Should().Be(command.ParentId);
    }
}
