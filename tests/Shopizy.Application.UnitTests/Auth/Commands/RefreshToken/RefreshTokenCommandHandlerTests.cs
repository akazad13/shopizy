using Moq;
using Shopizy.Application.Auth.Commands.RefreshToken;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPermissionRepository> _mockPermissionRepository;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IRefreshTokenGenerator> _mockRefreshTokenGenerator;
    private readonly Mock<IRefreshTokenStore> _mockRefreshTokenStore;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockRefreshTokenGenerator = new Mock<IRefreshTokenGenerator>();
        _mockRefreshTokenStore = new Mock<IRefreshTokenStore>();

        _mockRefreshTokenGenerator.Setup(g => g.Lifetime).Returns(TimeSpan.FromDays(7));

        _handler = new RefreshTokenCommandHandler(
            _mockUserRepository.Object,
            _mockPermissionRepository.Object,
            _mockJwtTokenGenerator.Object,
            _mockRefreshTokenGenerator.Object,
            _mockRefreshTokenStore.Object
        );
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenStoreReturnsNull_ShouldReturnInvalidCredentials()
    {
        // Arrange
        _mockRefreshTokenStore
            .Setup(s => s.ConsumeAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserId?)null);

        var command = new RefreshTokenCommand("invalid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Authentication.InvalidCredentials.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFound()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        _mockRefreshTokenStore
            .Setup(s => s.ConsumeAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        var command = new RefreshTokenCommand("valid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.User.UserNotFoundWhileLogin.Code);
    }

    [Fact]
    public async Task Handle_WhenValidToken_ShouldRefreshAndReturnNewAuthResult()
    {
        // Arrange
        var perm = Permission.Create("Read:Products");
        var user = User.Create(
            "John",
            "Doe",
            "john@example.com",
            "pass123",
            UserRole.Customer,
            new List<PermissionId> { perm.Id }
        );

        var permissions = new List<Permission> { perm };

        _mockRefreshTokenStore
            .Setup(s => s.ConsumeAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user.Id);
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(permissions);
        _mockJwtTokenGenerator
            .Setup(g => g.GenerateToken(user.Id, user.Role.ToString(), It.IsAny<List<string>>()))
            .Returns("new-access-token");
        _mockRefreshTokenGenerator.Setup(g => g.Generate()).Returns("new-refresh-token");

        var command = new RefreshTokenCommand("valid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Token.ShouldBe("new-access-token");
        result.Value.RefreshToken.ShouldBe("new-refresh-token");
        result.Value.Id.ShouldBe(user.Id.Value);
    }
}
