using ErrorOr;
using Moq;
using Shopizy.Application.Categories.Commands.DeleteCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly DeleteCategoryCommandHandler _sut;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public DeleteCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _sut = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    // Should delete the category and return a success response when the category exists
    [Fact]
    public async Task Should_DeleteCategoryAndReturnSuccessResponse_WhenCategoryExists()
    {
        // Arrange
        var command = DeleteCategoryCommandUtils.CreateCommand();
        var category = CategoryFactory.Create();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(command.CategoryId)))
            .ReturnsAsync(category);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.IsType<ErrorOr<Success>>(result);
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);
        Assert.Equal(Result.Success, result.Value);

        _mockCategoryRepository.Verify(x => x.Remove(category), Times.Once);
        _mockCategoryRepository.Verify(m => m.Commit(default), Times.Once);
    }
}
