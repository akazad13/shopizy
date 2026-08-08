using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.VerifyTwoFactor;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITotpHelper> _mockTotpHelper;
    private readonly VerifyTwoFactorCommandHandler _handler;

    public VerifyTwoFactorCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTotpHelper = new Mock<ITotpHelper>();
        _handler = new VerifyTwoFactorCommandHandler(
            _mockUserRepository.Object,
            _mockTotpHelper.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new VerifyTwoFactorCommand(Guid.NewGuid(), "123456");

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
    public async Task Handle_WhenTwoFactorNotSetup_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.CreateUser(); // TwoFactorSecret is null
        var command = new VerifyTwoFactorCommand(user.Id.Value, "123456");

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("TwoFactor.NotSetup");
    }

    [Fact]
    public async Task Handle_WhenCodeIsInvalid_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.EnableTwoFactor();
        var command = new VerifyTwoFactorCommand(user.Id.Value, "000000");

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _mockTotpHelper.Setup(t => t.VerifyCode(user.TwoFactorSecret!, "000000")).Returns(false);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("TwoFactor.InvalidCode");
        user.IsTwoFactorEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenCodeIsValid_ShouldConfirmTwoFactorAndReturnSuccess()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user.EnableTwoFactor();
        var command = new VerifyTwoFactorCommand(user.Id.Value, "123456");

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _mockTotpHelper.Setup(t => t.VerifyCode(user.TwoFactorSecret!, "123456")).Returns(true);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
        user.IsTwoFactorEnabled.ShouldBeTrue();
    }
}
