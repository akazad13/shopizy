using Moq;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Categories.Queries.GetCategory;

public class GetCategoryQueryHandlerTests
{
    private readonly GetCategoryQueryHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public GetCategoryQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new GetCategoryQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnCategoryDetails_WhenCategoryIsFound()
    {
        // Arrange
        var category = CategoryFactory.Create();
        var query = GetCategoryQueryUtils.CreateQuery(category.Id);
        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(query.CategoryId, result.Value.Id.Value);
        Assert.Equal(Constants.Category.Name, result.Value.Name);
        Assert.Equal(Constants.Category.ParentId, result.Value.ParentId);
    }

    [Fact]
    public async Task ShouldReturnCategoryNotFoundErrorWhenCategoryIdDoesNotExist()
    {
        // Arrange
        var query = GetCategoryQueryUtils.CreateQuery(Constants.Category.Id);
        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
            .ReturnsAsync(() => null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.True(result.IsError);
        Assert.Single(result.Errors);
        Assert.Equal(CustomErrors.Category.CategoryNotFound, result.Errors[0]);
    }
}
