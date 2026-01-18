using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandlerTests
{
    private readonly UpdatePasswordCommandHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordManager> _mockPasswordManager;

    public UpdatePasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordManager = new Mock<IPasswordManager>();
        _sut = new UpdatePasswordCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordManager.Object
        );
    }

    [Fact]
    public async Task Should_ReturnError_WhenUserIsNotFound()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserByIdAsync(UserId.Create(command.UserId)))
            .ReturnsAsync(() => null);

        _mockUserRepository.Setup(u => u.Update(user));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.User.UserNotFound, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserByIdAsync(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnError_WhenOldPasswordDoesNotMatch()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);
        _mockPasswordManager.Setup(x => x.Verify("oldPassword", "password")).Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.User.PasswordNotCorrect, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserByIdAsync(UserId.Create(command.UserId)), Times.Once);
        _mockPasswordManager.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once
        );
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
    }

    [Fact]
    public async Task Should_UpdatePassword_WhenOldPasswordIsCorrectAndNewPasswordIsProvided()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserByIdAsync(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockPasswordManager
            .Setup(u => u.Verify(user.Password ?? "", Constants.User.Password))
            .Returns(true);

        _mockPasswordManager
            .Setup(u => u.CreateHashString(Constants.User.NewPassword, 10000))
            .Returns("hashstring");

        _mockUserRepository.Setup(u => u.Update(user));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockUserRepository.Verify(x => x.GetUserByIdAsync(It.IsAny<UserId>()), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnError_WhenNewPasswordIsSameAsOldPassword()
    {
        // Arrange
        var command = UpdatePasswordCommandUtils.CreateCommandWithSameOldAndNewPassword();
        var user = UserFactory.CreateUser();

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);

        _mockPasswordManager
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _mockPasswordManager
            .Setup(u => u.CreateHashString(It.IsAny<string>(), 10000))
            .Returns("hashstring");

        _mockUserRepository.Setup(u => u.Update(user));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.IsType<ErrorOr<Success>>(result);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(CustomErrors.User.PasswordSameAsOld, result.Errors[0]);

        _mockUserRepository.Verify(x => x.GetUserByIdAsync(It.IsAny<UserId>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
    }
}
