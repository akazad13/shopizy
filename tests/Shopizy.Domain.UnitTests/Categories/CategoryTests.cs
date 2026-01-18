using Xunit;
using Shouldly;
using Shopizy.Domain.Categories;

namespace Shopizy.Domain.UnitTests.Categories;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var name = "Electronics";
        Guid? parentId = null;

        // Act
        var category = Category.Create(name, parentId);

        // Assert
        category.ShouldNotBeNull();
        category.Name.ShouldBe(name);
        category.ParentId.ShouldBe(parentId);
        category.Id.ShouldNotBeNull();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        var category = Category.Create("Old Name", null);
        var newName = "New Name";
        var newParentId = Guid.NewGuid();

        // Act
        category.Update(newName, newParentId);

        // Assert
        category.Name.ShouldBe(newName);
        category.ParentId.ShouldBe(newParentId);
    }
}
