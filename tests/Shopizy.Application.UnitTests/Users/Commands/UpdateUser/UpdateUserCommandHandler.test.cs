using ErrorOr;
using Moq;
using Shopizy.Application.Common.Caching;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdateUser;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly UpdateUserCommandHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICacheHelper> _mockCacheHelper;

    public UpdateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCacheHelper = new Mock<ICacheHelper>();
        _sut = new UpdateUserCommandHandler(
            _mockUserRepository.Object,
            _mockCacheHelper.Object
        );
    }

    [Fact]
    public async Task Should_UpdateUserSuccessfully_When_ValidDataProvided()
    {
        // Arrange
        var command = UpdateUserCommandUtils.CreateCommand();
        var user = UserFactory.CreateUser();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(user));
        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCacheHelper
            .Setup(c => c.RemoveAsync($"user-{user.Id.Value}"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheHelper.Verify(c => c.RemoveAsync($"user-{user.Id.Value}"), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnUserNotFound_When_UserDoesNotExist()
    {
        // Arrange
        var command = UpdateUserCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.User.UserNotFound, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _mockCacheHelper.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnUserNotUpdated_When_RepositoryCommitFails()
    {
        // Arrange
        var command = UpdateUserCommandUtils.CreateCommand();
        var user = UserFactory.CreateUser();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(user));
        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.User.UserNotUpdated, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheHelper.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Should_InvalidateCache_When_UserIsUpdatedSuccessfully()
    {
        // Arrange
        var command = UpdateUserCommandUtils.CreateCommand();
        var user = UserFactory.CreateUser();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(user));
        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mockCacheHelper
            .Setup(c => c.RemoveAsync($"user-{user.Id.Value}"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        _mockCacheHelper.Verify(c => c.RemoveAsync($"user-{user.Id.Value}"), Times.Once);
    }
}
