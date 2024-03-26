using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Categories.Commands.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;

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

        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(createCategoryCommand, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ValidateCreatedForm(createCategoryCommand);
    }
}
