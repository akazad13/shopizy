using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateAddress;

public class UpdateAddressCommandHandlerTests
{
    private readonly UpdateAddressCommandHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public UpdateAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _sut = new UpdateAddressCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnErrorResponse_WhenUserIsNotFound()
    {
        // Arrange
        var command = UpdateAddressCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(repo => repo.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Equal(CustomErrors.User.UserNotFound, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_UpdateUserAddressSuccessfully_WhenUserIsFound()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var command = UpdateAddressCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(user));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_HandleConcurrentUpdatesToTheSameUser()
    {
        // Arrange

        var command = UpdateAddressCommandUtils.CreateCommand();
        var user = UserFactory.CreateUser();

        _mockUserRepository
            .Setup(repo => repo.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(It.IsAny<User>()));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var task1 = _sut.Handle(command, CancellationToken.None);
        var task2 = _sut.Handle(command, CancellationToken.None);

        var result1 = await task1;
        var result2 = await task2;

        // Assert
        Assert.IsType<ErrorOr<Success>>(result1);
        Assert.IsType<ErrorOr<Success>>(result2);
        Assert.False(result1.IsError);
        Assert.False(result2.IsError);

        Assert.IsType<Success>(result1.Value);

        Assert.IsType<Success>(result2.Value);
    }

    [Fact]
    public async Task Should_HandleCases_WhereDatabaseConnectionIsLostDuringCommit()
    {
        // Arrange

        var command = UpdateAddressCommandUtils.CreateCommand();
        var user = UserFactory.CreateUser();

        _mockUserRepository.Setup(repo => repo.GetUserById(It.IsAny<UserId>())).ReturnsAsync(user);

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // Simulate database connection loss

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.User.UserNotUpdated, result.Errors[0]);
    }
}
