using ErrorOr;
using Moq;
using Shouldly;
using Shopizy.Application.Categories.Commands.UpdateCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Application.UnitTests.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly UpdateCategoryCommandHandler _sut;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _sut = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnCategory_WhenCategoryIsUpdatedSuccessfully()
    {
        // Arrange
        var command = UpdateCategoryCommandUtils.CreateCommand();
        var category = CategoryFactory.Create();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(command.CategoryId)))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
    }
}
