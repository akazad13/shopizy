using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Categories.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public DeleteCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async void DeleteCategory_WhenCategoryIsValid_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var deleteCategoryCommand = DeleteCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(deleteCategoryCommand.CategoryId)))
            .ReturnsAsync(Category.Create(Constants.Category.Name, Constants.Category.ParentId));
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(deleteCategoryCommand, default);

        // Assert
        result.IsError.Should().BeFalse();
    }
}
