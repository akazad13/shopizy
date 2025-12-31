using Moq;
using Shouldly;
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
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.ValidateResult(command);
    }

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
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldNotBeEmpty();
        result.Errors[0].ShouldBe(CustomErrors.Category.DuplicateName);
    }
}
