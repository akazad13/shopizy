using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandlerTests
{
    private readonly UpdatePasswordCommandHandler _handler;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordManager> _mockPasswordManager;

    public UpdatePasswordCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordManager = new Mock<IPasswordManager>();
        _handler = new UpdatePasswordCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordManager.Object
        );
    }

    [Fact]
    public async Task UpdatePassword_WhenUserIsNotFound_ReturnError()
    {
        // Arrange
        Domain.Users.User user = UserFactory.CreateUser();
        Domain.Users.User updatedUser = UserFactory.UpdatePassword(user);
        UpdatePasswordCommand query = UpdatePasswordCommandUtils.CreateCommand();

        _ = _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(() => null);

        _ = _mockUserRepository.Setup(u => u.Update(updatedUser));

        _ = _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _handler.Handle(query, default);

        // Assert
        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(query.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _ = result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePassword_WhenPasswordIsNotMatched_ReturnError()
    {
        // Arrange
        Domain.Users.User user = UserFactory.CreateUser();
        Domain.Users.User updatedUser = UserFactory.UpdatePassword(user);
        UpdatePasswordCommand query = UpdatePasswordCommandUtils.CreateCommand();

        _ = _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        _ = _mockPasswordManager
            .Setup(u => u.Verify(user.Password ?? "", Constants.User.NewPassword))
            .Returns(false);

        _ = _mockUserRepository.Setup(u => u.Update(updatedUser));

        _ = _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _handler.Handle(query, default);

        // Assert
        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(query.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Never);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _ = result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePassword_WhenUserIsFound_UpdateAndReturnSuccess()
    {
        // Arrange
        Domain.Users.User user = UserFactory.CreateUser();
        Domain.Users.User updatedUser = UserFactory.UpdatePassword(user);
        UpdatePasswordCommand query = UpdatePasswordCommandUtils.CreateCommand();

        _ = _mockUserRepository
            .Setup(u => u.GetUserById(UserId.Create(query.UserId)))
            .ReturnsAsync(user);

        _ = _mockPasswordManager
            .Setup(u => u.Verify(user.Password ?? "", Constants.User.NewPassword))
            .Returns(true);

        _ = _mockPasswordManager
            .Setup(u => u.CreateHashString(Constants.User.NewPassword, 10000))
            .Returns("hashstring");

        _ = _mockUserRepository.Setup(u => u.Update(updatedUser));

        _ = _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        ErrorOr.ErrorOr<ErrorOr.Success> result = await _handler.Handle(query, default);

        // Assert
        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(query.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _ = result.IsError.Should().BeFalse();
    }
}
