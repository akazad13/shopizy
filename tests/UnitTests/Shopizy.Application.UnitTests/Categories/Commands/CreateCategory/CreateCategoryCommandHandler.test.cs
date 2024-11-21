using FluentAssertions;
using Moq;
using Shopizy.Application.Categories.Commands.CreateCategory;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Categories.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.CustomErrors;

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
    public async Task ShouldCreateANewCategorySuccessfullyAsync()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();
        _mockCategoryRepository
            .Setup(x => x.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(false);
        _mockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        _mockCategoryRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.ValidateResult(command);
    }

    // Should return an error when trying to create a category with a duplicate name
    [Fact]
    public async Task ShouldReturnErrorWhenCreatingCategoryWithDuplicateNameAsync()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(true);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.Category.DuplicateName);
    }

    // Should return an category not created error when category save failed
    [Fact]
    public async Task ShouldReturnCategoryNotCreatedErrorWhenCategorySaveFailedAsync()
    {
        // Arrange
        var command = CreateCategoryCommandUtils.CreateCommand();

        _mockCategoryRepository
            .Setup(c => c.GetCategoryByNameAsync(command.Name))
            .ReturnsAsync(false);
        _mockCategoryRepository.Setup(c => c.Commit(default)).ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.Category.CategoryNotCreated);
    }

    // Should handle concurrent category creation requests without data corruption
    // [Fact]
    // public async Task ShouldHandleConcurrentCategoryCreationRequestsWithoutDataCorruptionAsync()
    // {
    //     // Arrange
    //     var command1 = new CreateCategoryCommand { Name = "Concurrent Category 1", ParentId = 1 };
    //     var command2 = new CreateCategoryCommand { Name = "Concurrent Category 2", ParentId = 1 };

    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(It.IsAny<string>()))
    //         .ReturnsAsync(false);
    //     _categoryRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Category>()))
    //         .Returns(Task.CompletedTask);
    //     _categoryRepositoryMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var task1 = _sut.Handle(command1, CancellationToken.None);
    //     var task2 = _sut.Handle(command2, CancellationToken.None);

    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     var result1 = task1.Result;
    //     var result2 = task2.Result;

    //     result1.ShouldNotBeNull();
    //     result1.IsSuccess.ShouldBeTrue();
    //     result1.Value.Name.ShouldBe(command1.Name);
    //     result1.Value.ParentId.ShouldBe(command1.ParentId);

    //     result2.ShouldNotBeNull();
    //     result2.IsSuccess.ShouldBeTrue();
    //     result2.Value.Name.ShouldBe(command2.Name);
    //     result2.Value.ParentId.ShouldBe(command2.ParentId);
    // }

    // // Should return an error when trying to create a category with a parent that does not exist
    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithNonExistingParentAsync()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Test Category", ParentId = 999999 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByIdAsync(command.ParentId))
    //         .ReturnsAsync((Category)null);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.ParentNotFound);
    // }

    // // Should validate the input category name length and return an error if it exceeds the maximum allowed length
    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithExceedingNameLength()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = new string('a', 101), ParentId = 1 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.InvalidNameLength);
    // }

    // // Should validate the input category name format and return an error if it contains invalid characters
    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithInvalidNameFormat()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Invalid@Category", ParentId = 1 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.InvalidNameFormat);
    // }

    // // Should handle a scenario where the database connection is lost during category creation
    // [Fact]
    // public async Task ShouldHandleDatabaseConnectionLostDuringCategoryCreation()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Test Category", ParentId = 1 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);
    //     _categoryRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Category>()))
    //         .Returns(Task.CompletedTask);
    //     _categoryRepositoryMock
    //         .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new Exception("Database connection lost"));

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.CategoryNotCreated);
    // }

    // // Should handle a scenario where the category repository throws an unexpected exception during category creation
    // [Fact]
    // public async Task ShouldHandleExceptionDuringCategoryCreation()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Test Category", ParentId = 1 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);
    //     _categoryRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Category>()))
    //         .ThrowsAsync(new Exception("Unexpected error"));

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.CategoryNotCreated);
    // }

    // // Should return an error when trying to create a category with a null or empty name
    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithNullOrEmptyName()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "", ParentId = 1 };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.InvalidName);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithNullOrEmptyParentId()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Test Category", ParentId = null };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.InvalidParentId);
    // }

    // // Should return an error when trying to create a category with a null or empty parent ID
    // [Fact]
    // public async Task ShouldReturnErrorWhenCreatingCategoryWithEmptyParentId()
    // {
    //     // Arrange
    //     var command = new CreateCategoryCommand { Name = "Test Category", ParentId = string.Empty };
    //     _categoryRepositoryMock
    //         .Setup(x => x.GetCategoryByNameAsync(command.Name))
    //         .ReturnsAsync(false);

    //     var _sut = new CreateCategoryCommand_sut(_categoryRepositoryMock.Object);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.IsSuccess.ShouldBeFalse();
    //     result.Errors.ShouldContain(CustomErrors.Category.InvalidParentId);
    // }
}
