using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Infrastructure.Categories.Specifications;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Categories.Specifications;

public class CategoryByIdSpecTests
{
    [Fact]
    public void Constructor_SetsCriteria()
    {
        // Arrange
        var categoryId = CategoryId.CreateUnique();

        // Act
        var spec = new CategoryByIdSpec(categoryId);

        // Assert
        spec.Criteria.ShouldNotBeNull();
    }
}
