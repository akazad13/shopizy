using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.EnableTwoFactor;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Users.Commands.EnableTwoFactor;

public class EnableTwoFactorCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly EnableTwoFactorCommandHandler _handler;

    public EnableTwoFactorCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new EnableTwoFactorCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = new EnableTwoFactorCommand(Guid.NewGuid());

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
    public async Task Handle_WhenUserExists_ShouldGenerateSecretAndQrUri()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var command = new EnableTwoFactorCommand(user.Id.Value);

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Secret.ShouldNotBeNullOrEmpty();
        result.Value.QrCodeUri.ShouldContain(user.Email);
        result.Value.QrCodeUri.ShouldContain(result.Value.Secret);
        user.TwoFactorSecret.ShouldBe(result.Value.Secret);
    }
}
