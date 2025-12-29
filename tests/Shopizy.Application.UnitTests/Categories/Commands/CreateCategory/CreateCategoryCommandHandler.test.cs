using Moq;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Xunit;

namespace Shopizy.Application.UnitTests.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CreateCategoryCommandHandler _sut;

    public CreateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _sut = new CreateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    // Should create a new category successfully
    [Fact]
    public async Task Should_CreateANewCategory_Successfully()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();
        _mockCategoryRepository
            .Setup(x => x.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(false);
        _mockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        result.Value.ValidateResult(command);
    }

    // Should return an error when trying to create a category with a duplicate name
    [Fact]
    public async Task Should_ReturnError_WhenCreatingCategoryWithDuplicateName()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.Category.DuplicateName, result.Errors[0]);
    }

    // Should return an category not created error when category save failed
}
