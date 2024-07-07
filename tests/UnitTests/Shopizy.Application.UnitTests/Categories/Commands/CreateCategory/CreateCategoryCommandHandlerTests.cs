using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.UnitTests.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly CreateCategoryCommandHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public CreateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async void CreateCategory_WhenCategoryIsValid_ShouldCrateAndReturnCategory()
    {
        // Arrange
        var createCategoryCommand = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(createCategoryCommand.Name))
            .ReturnsAsync(false);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(createCategoryCommand, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ValidateResult(createCategoryCommand);
    }

    [Fact]
    public async void CreateCategory_WhenCategoryNameIsExist_ShouldReturnDuplicateCategoryError()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(true);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().BeOfType(typeof(List<ErrorOr.Error>));
        result.Errors.First().Should().Be(CustomErrors.Category.DuplicateName);
    }

    [Fact]
    public async void CreateCategory_WhenCategorySaveFailed_ShouldReturnCategoryNotCreatedError()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(false);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().BeOfType(typeof(List<ErrorOr.Error>));
        result.Errors.First().Should().Be(CustomErrors.Category.CategoryNotCreated);
    }
}
