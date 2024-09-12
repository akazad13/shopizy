using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandlerTests
{
    private readonly UpdateCategoryCommandHandler _handler;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task UpdateCategory_WhenCategoryIsFound_UpdateAndReturnCategory()
    {
        // Arrange
        UpdateCategoryCommand command = UpdateCategoryCommandUtils.CreateCommand();

        _ = _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(command.CategoryId)))
            .ReturnsAsync(Category.Create(command.Name, command.ParentId));
        _ = _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<Category> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        result.Value.ValidateResult(command);
    }
}
