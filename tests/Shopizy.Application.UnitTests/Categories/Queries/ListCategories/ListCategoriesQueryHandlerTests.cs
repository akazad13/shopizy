using Moq;
using Shouldly;
using Shopizy.Application.Categories.Queries.ListCategories;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;

namespace Shopizy.Application.UnitTests.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly ListCategoriesQueryHandler _handler;

    public ListCategoriesQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new ListCategoriesQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenNoCategories_ReturnsEmptyList()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(x => x.GetCategoriesAsync())
            .ReturnsAsync(new List<Category>());

        var query = new ListCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create("Cat 1", null),
            Category.Create("Cat 2", null)
        };

        _mockCategoryRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(categories);

        var query = new ListCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(2);
        result.Value.ShouldContain(x => x.Name == "Cat 1");
        result.Value.ShouldContain(x => x.Name == "Cat 2");
    }
}
