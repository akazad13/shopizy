using System.Linq.Expressions;
using Shopizy.Domain.Common.Enums;
using Shopizy.Infrastructure.Common.Specifications;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Common.Specifications;

public class SpecificationTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestSpecification : Specification<TestEntity>
    {
        public TestSpecification() : base() { }
        public TestSpecification(Expression<Func<TestEntity, bool>> criteria) : base(criteria) { }

        public void TestAddCriteria(Expression<Func<TestEntity, bool>> predict) => AddCriteria(predict);
        public void TestAddPaging(int pageSize, int pageNumber) => AddPaging(pageSize, pageNumber);
        public void TestAddInclude(Expression<Func<TestEntity, object>> table) => AddInclude(table);
        public void TestAddOrderBy(Expression<Func<TestEntity, object>> orderByExpression, OrderType orderType) => AddOrderBy(orderByExpression, orderType);
    }

    [Fact]
    public void Constructor_WithCriteria_SetsCriteria()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> criteria = x => x.Id == 1;

        // Act
        var spec = new TestSpecification(criteria);

        // Assert
        spec.Criteria.ShouldBe(criteria);
    }

    [Fact]
    public void AddCriteria_WhenCriteriaIsNull_SetsCriteria()
    {
        // Arrange
        var spec = new TestSpecification();
        Expression<Func<TestEntity, bool>> predict = x => x.Id == 1;

        // Act
        spec.TestAddCriteria(predict);

        // Assert
        spec.Criteria.ShouldBe(predict);
    }

    [Fact]
    public void AddCriteria_WhenCriteriaIsNotNull_CombinesCriteriaWithAndAlso()
    {
        // Arrange
        var spec = new TestSpecification(x => x.Id == 1);
        Expression<Func<TestEntity, bool>> predict = x => x.Name == "Test";

        // Act
        spec.TestAddCriteria(predict);

        // Assert
        spec.Criteria.ShouldNotBeNull();
        var func = spec.Criteria.Compile();
        func(new TestEntity { Id = 1, Name = "Test" }).ShouldBeTrue();
        func(new TestEntity { Id = 1, Name = "Other" }).ShouldBeFalse();
        func(new TestEntity { Id = 2, Name = "Test" }).ShouldBeFalse();
    }

    [Fact]
    public void AddPaging_SetsSkipAndTake()
    {
        // Arrange
        var spec = new TestSpecification();

        // Act
        spec.TestAddPaging(10, 2);

        // Assert
        spec.Skip.ShouldBe(10);
        spec.Take.ShouldBe(10);
        spec.IsPagingEnabled.ShouldBeTrue();
    }

    [Fact]
    public void AddOrderBy_SetsOrderByExpression()
    {
        // Arrange
        var spec = new TestSpecification();

        // Act
        spec.TestAddOrderBy(x => x.Name, OrderType.Ascending);

        // Assert
        spec.OrderByExpression.ShouldNotBeNull();
        spec.OrderByDescendingExpression.ShouldBeNull();
    }

    [Fact]
    public void AddOrderByDescending_SetsOrderByDescendingExpression()
    {
        // Arrange
        var spec = new TestSpecification();

        // Act
        spec.TestAddOrderBy(x => x.Name, OrderType.Descending);

        // Assert
        spec.OrderByDescendingExpression.ShouldNotBeNull();
        spec.OrderByExpression.ShouldBeNull();
    }
}
