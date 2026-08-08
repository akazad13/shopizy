using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.DisableTwoFactor;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.DisableTwoFactor;

public class DisableTwoFactorCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DisableTwoFactorCommandHandler _handler;

    public DisableTwoFactorCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new DisableTwoFactorCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new DisableTwoFactorCommand(Guid.NewGuid());

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((Shopizy.Domain.Users.User?)null);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.User.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldDisableTwoFactorAndReturnSuccess()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.EnableTwoFactor();
        user.ConfirmTwoFactor();
        user.IsTwoFactorEnabled.ShouldBeTrue();

        var command = new DisableTwoFactorCommand(user.Id.Value);

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
        user.IsTwoFactorEnabled.ShouldBeFalse();
        user.TwoFactorSecret.ShouldBeNull();
    }
}
