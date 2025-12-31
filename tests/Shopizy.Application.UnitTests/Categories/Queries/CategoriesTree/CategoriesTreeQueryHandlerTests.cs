using ErrorOr;
using Moq;
using Shouldly;
using Shopizy.Application.Categories.Queries.CategoriesTree;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.Queries.CategoriesTree;

public class CategoriesTreeQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoriesTreeQueryHandler _handler;

    public CategoriesTreeQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new CategoriesTreeQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenNoCategories_ReturnsEmptyTree()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(x => x.GetCategoriesAsync())
            .ReturnsAsync(new List<Category>());

        var query = new CategoriesTreeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ReturnsCorrectTreeStructure()
    {
        // Arrange
        var root1 = Category.Create("Root 1", null);
        var sub1 = Category.Create("Sub 1", root1.Id.Value);
        var root2 = Category.Create("Root 2", null);

        var categories = new List<Category> { root1, sub1, root2 };

        _mockCategoryRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(categories);

        var query = new CategoriesTreeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(2);
        
        var root1Node = result.Value.First(x => x.Id == root1.Id.Value);
        root1Node.Children.Count.ShouldBe(1);
        root1Node.Children[0].Id.ShouldBe(sub1.Id.Value);
        
        var root2Node = result.Value.First(x => x.Id == root2.Id.Value);
        root2Node.Children.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDeepHierarchy_ReturnsCorrectTreeStructure()
    {
        // Arrange
        var root = Category.Create("Root", null);
        var level1 = Category.Create("L1", root.Id.Value);
        var level2 = Category.Create("L2", level1.Id.Value);

        var categories = new List<Category> { root, level1, level2 };

        _mockCategoryRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(categories);

        var query = new CategoriesTreeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Children.Count.ShouldBe(1);
        result.Value[0].Children[0].Children.Count.ShouldBe(1);
        result.Value[0].Children[0].Children[0].Id.ShouldBe(level2.Id.Value);
    }
}
