using Shopizy.Domain.Common.Enums;
using Shopizy.Infrastructure.Common.Specifications;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Common.Specifications;

public class SpecificationEvaluatorTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestSpecification : Specification<TestEntity>
    {
        public void ApplyCriteria(int id) => AddCriteria(x => x.Id == id);
        public void ApplyPaging(int skip, int take) => AddPaging(take, (skip / take) + 1);
        public void ApplyOrderBy(OrderType orderType) => AddOrderBy(x => x.Name, orderType);
    }

    [Fact]
    public void GetQuery_WithCriteria_AppliesFilter()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Id = 1, Name = "A" },
            new() { Id = 2, Name = "B" }
        }.AsQueryable();

        var spec = new TestSpecification();
        spec.ApplyCriteria(1);

        // Act
        var result = SpecificationEvaluator.GetQuery(data, spec).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(1);
    }

    [Fact]
    public void GetQuery_WithOrderBy_AppliesSorting()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Id = 1, Name = "B" },
            new() { Id = 2, Name = "A" }
        }.AsQueryable();

        var spec = new TestSpecification();
        spec.ApplyOrderBy(OrderType.Ascending);

        // Act
        var result = SpecificationEvaluator.GetQuery(data, spec).ToList();

        // Assert
        result[0].Name.ShouldBe("A");
        result[1].Name.ShouldBe("B");
    }

    [Fact]
    public void GetQuery_WithPaging_AppliesPaging()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 }
        }.AsQueryable();

        var spec = new TestSpecification();
        spec.ApplyPaging(1, 1); // Skip 1, Take 1

        // Act
        var result = SpecificationEvaluator.GetQuery(data, spec).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(2);
    }
}
