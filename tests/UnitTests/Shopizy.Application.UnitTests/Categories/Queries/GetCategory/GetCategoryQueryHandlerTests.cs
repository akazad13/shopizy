using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Queries.GetCategory;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Categories.Queries.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

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
    public async void GetCategory_WhenCategoryIsFound_ShouldReturnCategory()
    {
        // Arrange
        var getCategoryQuery = GetCategoryQueryUtils.CreateQuery();
        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(getCategoryQuery.CategoryId)))
            .ReturnsAsync(Category.Create(Constants.Category.Name, Constants.Category.ParentId));

        // Act
        var result = await _handler.Handle(getCategoryQuery, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value?.ValidateCreatedForm(getCategoryQuery);
    }
}