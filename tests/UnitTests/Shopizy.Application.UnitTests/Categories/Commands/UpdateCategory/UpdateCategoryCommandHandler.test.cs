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
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly UpdateCategoryCommandHandler _sut;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _sut = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenCategoryIdIsInvalid()
    // {
    //     // Arrange
    //     var invalidCategoryId = Guid.Empty;
    //     var command = new UpdateCategoryCommand(invalidCategoryId, "New Category", null);
    //     var handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync((Category)null);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(CustomErrors.Category.CategoryNotFound, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCategoryNotFound()
    // {
    //     // Arrange
    //     var invalidCategoryId = Guid.NewGuid();
    //     var command = new UpdateCategoryCommand(invalidCategoryId, "New Category", null);
    //     var categoryRepositoryMock = new Mock<ICategoryRepository>();
    //     var handler = new UpdateCategoryCommandHandler(categoryRepositoryMock.Object);

    //     categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync((Category)null);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(CustomErrors.Category.CategoryNotFound, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldUpdateTheCategoryNameAndParentIdCorrectly()
    // {
    //     // Arrange
    //     var existingCategoryId = Guid.NewGuid();
    //     var existingCategory = new Category(existingCategoryId, "Old Category", null);
    //     var newCategoryName = "New Category";
    //     var newParentId = Guid.NewGuid();
    //     var command = new UpdateCategoryCommand(
    //         existingCategoryId,
    //         newCategoryName,
    //         newParentId
    //     );
    //     var handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(existingCategory);

    //     _categoryRepositoryMock.Setup(repo => repo.Update(It.IsAny<Category>()));

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(existingCategoryId, result.Value.Id);
    //     Assert.Equal(newCategoryName, result.Value.Name);
    //     Assert.Equal(newParentId, result.Value.ParentId);
    //     _categoryRepositoryMock.Verify(
    //         repo =>
    //             repo.Update(
    //                 It.Is<Category>(c =>
    //                     c.Id == existingCategoryId
    //                     && c.Name == newCategoryName
    //                     && c.ParentId == newParentId
    //                 )
    //             ),
    //         Times.Once
    //     );
    //     _categoryRepositoryMock.Verify(
    //         repo => repo.Commit(It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldCallCommitMethodExactlyOnce()
    // {
    //     // Arrange
    //     var categoryRepositoryMock = new Mock<ICategoryRepository>();
    //     var command = new UpdateCategoryCommand(Guid.NewGuid(), "New Category", Guid.NewGuid());
    //     var handler = new UpdateCategoryCommandHandler(categoryRepositoryMock.Object);

    //     categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(new Category(Guid.NewGuid(), "Old Category", null));

    //     categoryRepositoryMock.Setup(repo => repo.Update(It.IsAny<Category>()));

    //     // Act
    //     await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     categoryRepositoryMock.Verify(
    //         repo => repo.Commit(It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    // }

    [Fact]
    public async Task ShouldReturnCategoryWhenCategoryIsUpdatedSuccessfullyAsync()
    {
        // Arrange
        var command = UpdateCategoryCommandUtils.CreateCommand();
        var category = CategoryFactory.Create();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(command.CategoryId)))
            .ReturnsAsync(category);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = (await _sut.Handle(command, default)).Match(x => x, x => null);

        // Assert
        result.Should().BeOfType(typeof(Category));
        result.Should().NotBeNull();
        result.ValidateResult(command);
    }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCategoryNotUpdatedDueToConcurrencyIssues()
    // {
    //     // Arrange
    //     var existingCategoryId = Guid.NewGuid();
    //     var existingCategory = new Category(existingCategoryId, "Old Category", null);
    //     var newCategoryName = "New Category";
    //     var newParentId = Guid.NewGuid();
    //     var command = new UpdateCategoryCommand(
    //         existingCategoryId,
    //         newCategoryName,
    //         newParentId
    //     );
    //     var categoryRepositoryMock = new Mock<ICategoryRepository>();
    //     var handler = new UpdateCategoryCommandHandler(categoryRepositoryMock.Object);

    //     categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(existingCategory);

    //     categoryRepositoryMock.Setup(repo => repo.Update(It.IsAny<Category>()));

    //     categoryRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(0);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(CustomErrors.Category.CategoryNotUpdated, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsCorrectly()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var categoryName = "Original Category";
    //     var category = new Category(categoryId, categoryName, null);
    //     var categoryRepositoryMock = new Mock<ICategoryRepository>();
    //     var handler = new UpdateCategoryCommandHandler(categoryRepositoryMock.Object);

    //     categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(category);

    //     categoryRepositoryMock.Setup(repo => repo.Update(It.IsAny<Category>()));

    //     categoryRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     var updateTasks = new List<Task<IResult<Category>>>();
    //     for (int i = 0; i < 10; i++)
    //     {
    //         var command = new UpdateCategoryCommand(categoryId, $"Updated Category {i}", null);
    //         updateTasks.Add(handler.Handle(command, CancellationToken.None));
    //     }

    //     // Act
    //     var results = await Task.WhenAll(updateTasks);

    //     // Assert
    //     foreach (var result in results)
    //     {
    //         Assert.True(result.IsSuccess);
    //         Assert.Equal(categoryId, result.Value.Id);
    //         Assert.StartsWith("Updated Category", result.Value.Name);
    //         Assert.Null(result.Value.ParentId);
    //     }

    //     categoryRepositoryMock.Verify(
    //         repo => repo.Update(It.IsAny<Category>()),
    //         Times.Exactly(10)
    //     );

    //     categoryRepositoryMock.Verify(
    //         repo => repo.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(10)
    //     );
    // }

    // [Fact]
    // public void ShouldValidateInputCategoryNameAndParentId()
    // {
    //     // Arrange
    //     var categoryRepositoryMock = new Mock<ICategoryRepository>();
    //     var handler = new UpdateCategoryCommandHandler(categoryRepositoryMock.Object);
    //     var validCategoryId = Guid.NewGuid();
    //     var validCategoryName = "Valid Category";
    //     var validParentId = Guid.NewGuid();

    //     // Act
    //     var validCommand = new UpdateCategoryCommand(
    //         validCategoryId,
    //         validCategoryName,
    //         validParentId
    //     );
    //     var validResult = handler.Handle(validCommand, CancellationToken.None).Result;

    //     var invalidCategoryNameCommand = new UpdateCategoryCommand(
    //         validCategoryId,
    //         "",
    //         validParentId
    //     );
    //     var invalidCategoryNameResult = handler
    //         .Handle(invalidCategoryNameCommand, CancellationToken.None)
    //         .Result;

    //     var invalidParentIdCommand = new UpdateCategoryCommand(
    //         validCategoryId,
    //         validCategoryName,
    //         Guid.Empty
    //     );
    //     var invalidParentIdResult = handler
    //         .Handle(invalidParentIdCommand, CancellationToken.None)
    //         .Result;

    //     // Assert
    //     Assert.True(validResult.IsSuccess);
    //     Assert.False(invalidCategoryNameResult.IsSuccess);
    //     Assert.Contains(
    //         CustomErrors.Category.InvalidCategoryName,
    //         invalidCategoryNameResult.Errors
    //     );
    //     Assert.False(invalidParentIdResult.IsSuccess);
    //     Assert.Contains(CustomErrors.Category.InvalidParentId, invalidParentIdResult.Errors);
    // }

    // [Fact]
    // public async Task ShouldLogRelevantInformationForAuditingPurposes()
    // {
    //     // Arrange
    //     var existingCategoryId = Guid.NewGuid();
    //     var existingCategory = new Category(existingCategoryId, "Old Category", null);
    //     var newCategoryName = "New Category";
    //     var newParentId = Guid.NewGuid();
    //     var command = new UpdateCategoryCommand(
    //         existingCategoryId,
    //         newCategoryName,
    //         newParentId
    //     );
    //     var handler = new UpdateCategoryCommandHandler(
    //         _categoryRepositoryMock.Object,
    //         _loggerMock.Object
    //     );

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(existingCategory);

    //     _categoryRepositoryMock.Setup(repo => repo.Update(It.IsAny<Category>()));

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     // Act
    //     await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     _loggerMock.Verify(
    //         logger =>
    //             logger.Log(
    //                 LogLevel.Information,
    //                 It.IsAny<EventId>(),
    //                 It.Is<It.IsAnyType>((o, t) => true),
    //                 It.IsAny<Exception>(),
    //                 (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
    //             ),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenCategoryNameOrParentIdIsTooLong()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var tooLongCategoryName = new string('a', 101); // Assuming maximum length is 100
    //     var tooLongParentId = Guid.NewGuid();
    //     var command = new UpdateCategoryCommand(
    //         categoryId,
    //         tooLongCategoryName,
    //         tooLongParentId
    //     );
    //     var handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(new Category(categoryId, "Old Category", null));

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Contains(CustomErrors.Category.CategoryNameTooLong, result.Errors);
    //     Assert.Contains(CustomErrors.Category.ParentIdTooLong, result.Errors);
    // }
}
