using ErrorOr;
using FluentAssertions;
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

    // [Fact]
    // public async Task ShouldThrowExceptionWhenUserIdIsNotProvided()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "",
    //         OldPassword = "oldPassword",
    //         NewPassword = "newPassword",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.UserNotFound, result.Errors);
    // }

    [Fact]
    public async Task ShouldReturnErrorWhenUserIsNotFoundAsync()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var updatedUser = UserFactory.UpdatePassword(user);
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(() => null);

        _mockUserRepository.Setup(u => u.Update(updatedUser));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.User.UserNotFound);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenOldPasswordDoesNotMatchAsync()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository.Setup(x => x.GetUserById(It.IsAny<UserId>())).ReturnsAsync(user);
        _mockPasswordManager.Setup(x => x.Verify("oldPassword", "password")).Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.User.PasswordNotCorrect);

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockPasswordManager.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once
        );
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldUpdatePasswordWhenOldPasswordIsCorrectAndNewPasswordIsProvidedAsync()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var updatedUser = UserFactory.UpdatePassword(user);
        var command = UpdatePasswordCommandUtils.CreateCommand();

        _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(command.UserId)))
            .ReturnsAsync(user);

        _mockPasswordManager
            .Setup(u => u.Verify(user.Password ?? "", Constants.User.NewPassword))
            .Returns(true);

        _mockPasswordManager
            .Setup(u => u.CreateHashString(Constants.User.NewPassword, 10000))
            .Returns("hashstring");

        _mockUserRepository.Setup(u => u.Update(updatedUser));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        result.Value.Should().NotBeNull();

        _mockUserRepository.Verify(x => x.GetUserById(It.IsAny<UserId>()), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordIsSameAsOldPassword()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "123",
    //         OldPassword = "oldPassword",
    //         NewPassword = "oldPassword",
    //     };
    //     userRepositoryMock
    //         .Setup(x => x.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(new Domain.Users.User(UserId.Create("123"), "oldPassword"));
    //     passwordManagerMock
    //         .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordCannotBeSame, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordIsTooShort()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "userId",
    //         OldPassword = "oldPassword",
    //         NewPassword = "new", // New password is too short
    //     };

    //     userRepositoryMock
    //         .Setup(x => x.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(new Domain.Users.User("userId", "username", "email", "password"));

    //     passwordManagerMock
    //         .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordTooShort, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordIsTooLong()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "123",
    //         OldPassword = "oldPassword",
    //         NewPassword = "newPassword".PadRight(101, 'a'), // Assuming a maximum length of 100 characters
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordTooLong, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordDoesNotContainUppercaseLetter()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     passwordManagerMock
    //         .Setup(pm => pm.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);
    //     passwordManagerMock
    //         .Setup(pm => pm.CreateHashString(It.IsAny<string>()))
    //         .Returns("HashedNewPassword");
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "userId",
    //         OldPassword = "oldPassword",
    //         NewPassword = "newpassword",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordNotValid, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordDoesNotContainLowercaseLetter()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     passwordManagerMock
    //         .Setup(pm => pm.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);
    //     passwordManagerMock
    //         .Setup(pm => pm.CreateHashString(It.IsAny<string>()))
    //         .Returns("HashedPassword");
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "123",
    //         OldPassword = "oldPassword",
    //         NewPassword = "NEWPASSWORD",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordNotValid, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordDoesNotContainNumericDigit()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     passwordManagerMock
    //         .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);
    //     passwordManagerMock
    //         .Setup(x => x.CreateHashString(It.IsAny<string>()))
    //         .Returns("hashedPassword");
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "userId",
    //         OldPassword = "oldPassword",
    //         NewPassword = "newPassword",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordNotValid, result.Errors);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenNewPasswordDoesNotContainSpecialCharacter()
    // {
    //     // Arrange
    //     var userRepositoryMock = new Mock<IUserRepository>();
    //     var passwordManagerMock = new Mock<IPasswordManager>();
    //     var handler = new UpdatePasswordCommandHandler(
    //         userRepositoryMock.Object,
    //         passwordManagerMock.Object
    //     );
    //     var command = new UpdatePasswordCommand
    //     {
    //         UserId = "123",
    //         OldPassword = "oldPassword",
    //         NewPassword = "newPassword", // Invalid password, no special character
    //     };

    //     userRepositoryMock
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(new Domain.Users.User("123", "username", "oldPasswordHash"));

    //     passwordManagerMock
    //         .Setup(pm => pm.Verify(It.IsAny<string>(), It.IsAny<string>()))
    //         .Returns(true);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(CustomErrors.User.PasswordNotValid, result.Errors);
    // }
}
