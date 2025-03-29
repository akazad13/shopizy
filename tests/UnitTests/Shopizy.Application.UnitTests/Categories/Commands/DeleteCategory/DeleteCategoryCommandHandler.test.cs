using ErrorOr;
using FluentAssertions;
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

    // Should throw an exception when the category ID is not provided
    // [Fact]
    // public void ShouldThrowExceptionWhenCategoryIdIsNotProvided()
    // {
    //     // Arrange
    //     var command = new DeleteCategoryCommand { CategoryId = Guid.Empty };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     // Act & Assert
    //     Assert.ThrowsAsync<ArgumentException>(
    //         async () => await sut.Handle(command, CancellationToken.None)
    //     );
    // }

    // Should return an error response when the category does not exist
    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenCategoryDoesNotExist()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(CategoryId.Create(categoryId)))
    //         .ReturnsAsync(null);

    //     // Act
    //     var result = await sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<ErrorResponse<Success>>(result);
    //     var errorResponse = (ErrorResponse<Success>)result;
    //     Assert.Equal(CustomErrors.Category.CategoryNotFound, errorResponse.Errors[0]);
    // }

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
        result.Should().BeOfType(typeof(ErrorOr<Success>));
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Success));
        result.Value.Should().Be(Result.Success);

        _mockCategoryRepository.Verify(x => x.Remove(category), Times.Once);
        _mockCategoryRepository.Verify(m => m.Commit(default), Times.Once);
    }

    // Should rollback the changes if the category deletion fails
    // [Fact]
    // public async Task ShouldRollbackChangesIfCategoryDeletionFailsAsync()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var category = new Category(CategoryId.Create(categoryId), "Test Category");

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(category);
    //     _categoryRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     _categoryRepositoryMock.Verify(x => x.Remove(category), Times.Once);
    //     _categoryRepositoryMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    //     Assert.Equal(
    //         Response<Success>.ErrorResponse([CustomErrors.Category.CategoryNotDeleted]),
    //         result
    //     );
    // }

    // // Should handle concurrent category deletion requests gracefully
    // [Fact]
    // public async Task ShouldHandleConcurrentDeletionGracefully()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var category = new Category(CategoryId.Create(categoryId)); // Assume Category class exists
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(category);

    //     _categoryRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var tasks = new List<Task>();

    //     for (int i = 0; i < 5; i++)
    //     {
    //         tasks.Add(sut.Handle(command, CancellationToken.None));
    //     }

    //     // Act
    //     await Task.WhenAll(tasks);

    //     // Assert
    //     _categoryRepositoryMock.Verify(
    //         x => x.Remove(It.Is<Category>(c => c.Id == category.Id)),
    //         Times.Exactly(5)
    //     );

    //     _categoryRepositoryMock.Verify(
    //         x => x.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(5)
    //     );
    // }

    // // Should validate the category ID format before processing
    // [Fact]
    // public void ShouldValidateCategoryIdFormatBeforeProcessing()
    // {
    //     // Arrange
    //     var command = new DeleteCategoryCommand { CategoryId = Guid.NewGuid() };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsCompletedSuccessfully);
    //     // Additional assertions can be added if needed
    // }

    // // Should log an error message when the category deletion fails

    // [Fact]
    // public async Task ShouldLogErrorMessageWhenCategoryDeletionFails()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var category = new Category(CategoryId.Create(categoryId));
    //     var mockCategoryRepository = new Mock<ICategoryRepository>();
    //     mockCategoryRepository
    //         .Setup(repo => repo.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(category);
    //     mockCategoryRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(0);
    //     var mockLogger = new Mock<ILogger<DeleteCategoryCommandHandler>>();
    //     var sut = new DeleteCategoryCommandHandler(mockCategoryRepository.Object);

    //     // Act
    //     var result = await sut.Handle(
    //         new DeleteCategoryCommand { CategoryId = categoryId },
    //         CancellationToken.None
    //     );

    //     // Assert
    //     mockLogger.Verify(
    //         logger =>
    //             logger.LogError(
    //                 It.IsAny<string>(),
    //                 It.Is<CategoryNotDeleted>(error => error.CategoryId == categoryId)
    //             ),
    //         Times.Once
    //     );
    // }

    // // Should return a timeout error when the category deletion takes longer than expected
    // [Fact]
    // public async Task ShouldReturnTimeoutErrorWhenDeletionTakesLongerThanExpected()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(new Domain.Categories.Category(categoryId, "Category1"));

    //     _categoryRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(() => Task.Delay(5000).Result); // Simulate a delay of 5 seconds

    //     // Act
    //     var result = await sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Success>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Category.CategoryNotDeleted, result.Errors.Single());
    // }

    // // Should handle database connection issues during category deletion
    // [Fact]
    // public async Task ShouldHandleDatabaseConnectionIssuesDuringCategoryDeletion()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(new Domain.Categories.Category(categoryId, "Test Category"));

    //     _categoryRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(-1); // Simulate database connection issue

    //     // Act
    //     var result = await sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Category.CategoryNotDeleted, result.Errors.First());
    // }

    // // Should support cancellation of category deletion requests
    // [Fact]
    // public async Task ShouldSupportCancellationOfCategoryDeletionRequests()
    // {
    //     // Arrange
    //     var categoryId = Guid.NewGuid();
    //     var command = new DeleteCategoryCommand { CategoryId = categoryId };
    //     var category = new Category(CategoryId.Create(categoryId), "Test Category");
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(It.IsAny<CategoryId>()))
    //         .ReturnsAsync(category);
    //     var cancellationTokenSource = new CancellationTokenSource();
    //     var sut = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);

    //     // Act
    //     var task = sut.Handle(command, cancellationTokenSource.Token);
    //     cancellationTokenSource.Cancel();

    //     // Assert
    //     await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
    // }
}
