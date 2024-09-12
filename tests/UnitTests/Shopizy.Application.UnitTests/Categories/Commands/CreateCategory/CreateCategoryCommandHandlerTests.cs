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
    public async Task CreateCategory_WhenCategoryIsValid_CrateAndReturnCategory()
    {
        // Arrange
        CreateCategoryCommand createCategoryCommand = CreateCategoryCommandUtils.CreateCommand();

        _ = _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(createCategoryCommand.Name))
            .ReturnsAsync(false);
        _ = _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<Domain.Categories.Category> result = await _handler.Handle(createCategoryCommand, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        result.Value.ValidateResult(createCategoryCommand);
    }

    [Fact]
    public async Task CreateCategory_WhenCategoryNameIsExist_ReturnDuplicateCategoryError()
    {
        // Arrange
        CreateCategoryCommand command = CreateCategoryCommandUtils.CreateCommand();

        _ = _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(true);
        _ = _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<Domain.Categories.Category> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeTrue();
        _ = result.Errors.Should().BeOfType(typeof(List<ErrorOr.Error>));
        _ = result.Errors.First().Should().Be(CustomErrors.Category.DuplicateName);
    }

    [Fact]
    public async Task CreateCategory_WhenCategorySaveFailed_ReturnCategoryNotCreatedError()
    {
        // Arrange
        CreateCategoryCommand command = CreateCategoryCommandUtils.CreateCommand();

        _ = _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(false);
        _ = _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(0);

        // Act
        ErrorOr.ErrorOr<Domain.Categories.Category> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeTrue();
        _ = result.Errors.Should().BeOfType(typeof(List<ErrorOr.Error>));
        _ = result.Errors.First().Should().Be(CustomErrors.Category.CategoryNotCreated);
    }
}
