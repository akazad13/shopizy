using FluentAssertions;
using Moq;
using Shopizy.Application.Auth.Common;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Auth.TestUtils;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Permissions;
using Shopizy.Domain.Users;

namespace Shopizy.Application.UnitTests.Auth.Queries.login;

public class LoginQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPermissionRepository> _mockPermissionRepository;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IPasswordManager> _mockPasswordManager;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordManager = new Mock<IPasswordManager>();
        _mockCartRepository = new Mock<ICartRepository>();

        _handler = new LoginQueryHandler(
            _mockUserRepository.Object,
            _mockPermissionRepository.Object,
            _mockJwtTokenGenerator.Object,
            _mockPasswordManager.Object,
            _mockCartRepository.Object
        );
    }

    [Fact]
    public async Task Should_ReturnsUserNotFoundWhileLoginError_WhenUserNotFound()
    {
        // Arrange
        var query = new LoginQuery("test@test.com", "password");
        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(query.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomErrors.User.UserNotFoundWhileLogin);
    }

    [Fact]
    public async Task ShouldReturnsInvalidCredentialsErrorWhenInvalidPassword()
    {
        // Arrange
        var query = new LoginQuery("test@test.com", "password");
        var user = User.Create("John", "Doe", "test@test.com", "password", []);

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password)).Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomErrors.Authentication.InvalidCredentials);
    }

    [Fact]
    public async Task Should_GeneratesTokenWithCorrectData_WhenValidCredentials()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        var permissions = AuthFactory.GetPermissions();
        var expectedToken = "generated_token";

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password!)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(permissions);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(
                    user.Id,
                    It.IsAny<List<string>>(),
                    It.Is<IEnumerable<string>>(p =>
                        p.SequenceEqual(new[] { permissions[0].Name, permissions[1].Name })
                    )
                )
            )
            .Returns(expectedToken);
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result
            .Value.Should()
            .BeEquivalentTo(
                new AuthResult(
                    user.Id.Value,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    expectedToken
                )
            );
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    It.Is<List<string>>(r => r.Count == 0),
                    It.Is<IEnumerable<string>>(p =>
                        p.SequenceEqual(new[] { permissions[0].Name, permissions[1].Name })
                    )
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnsAuthResultWithEmptyPermissions_WhenEmptyPermissionList()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password!)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync([]);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, It.IsAny<List<string>>(), It.IsAny<IEnumerable<string>>())
            )
            .Returns("generatedToken");
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<AuthResult>();
        result.Value.Id.Should().Be(user.Id.Value);
        result.Value.FirstName.Should().Be(user.FirstName);
        result.Value.LastName.Should().Be(user.LastName);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Token.Should().Be("generatedToken");
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    It.Is<List<string>>(r => r.Count == 0),
                    It.Is<IEnumerable<string>>(p => !p.Any())
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnsAuthResultWithCorrectDetails_WhenValidCredentials()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        var permissions = AuthFactory.GetPermissions();

        var token = "generatedToken";

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password!)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(permissions);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, It.IsAny<List<string>>(), It.IsAny<IEnumerable<string>>())
            )
            .Returns(token);
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<AuthResult>();
        result.Value.Id.Should().Be(user.Id.Value);
        result.Value.FirstName.Should().Be(user.FirstName);
        result.Value.LastName.Should().Be(user.LastName);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Token.Should().Be(token);
    }

    [Fact]
    public async Task Should_ReturnsAuthResultWithEmptyPermissions_WhenUserWithNoAssignedPermissions()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        var allPermissions = new List<Permission> { Permission.Create("SomePermission") };

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password!)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(allPermissions);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, It.IsAny<List<string>>(), It.IsAny<IEnumerable<string>>())
            )
            .Returns("generatedToken");
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<AuthResult>();
        result.Value.Token.Should().Be("generatedToken");
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    It.Is<List<string>>(r => r.Count == 0),
                    It.Is<IEnumerable<string>>(p => !p.Any())
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnAuthResultWithAssignedPermissions_WhenCorrectlyFiltersAndSelectsAssignedPermissions()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        var allPermissions = AuthFactory.GetPermissions();

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(pm => pm.Verify(query.Password, user.Password)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(allPermissions);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, It.IsAny<List<string>>(), It.IsAny<IEnumerable<string>>())
            )
            .Returns("generatedToken");
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    It.Is<List<string>>(roles => roles.Count == 0),
                    It.Is<IEnumerable<string>>(permissions =>
                        permissions.Count() == 2
                        && permissions.Contains("get:category")
                        && permissions.Contains("get:product")
                    )
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ThrowsOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var query = new LoginQuery("test@test.com", "password");
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cancellationTokenSource.Token)
        );

        cancellationTokenSource.Dispose();
    }

    [Fact]
    public async Task Should_ReturnsInvalidCredentialsError_WhenNullPassword()
    {
        // Arrange
        var query = new LoginQuery("test@test.com", null!);
        var user = UserFactory.CreateUser();
        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager
            .Setup(pm => pm.Verify(It.IsAny<string>(), user.Password!))
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomErrors.Authentication.InvalidCredentials);
    }

    [Fact]
    public async Task Should_ReturnsCorrectAuthResultWithEmptyRoles_WhenValidCredentials()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        var permissions = AuthFactory.GetPermissions();

        var expectedToken = "generatedToken";

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(pm => pm.Verify(query.Password, user.Password)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(permissions);
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, It.IsAny<List<string>>(), It.IsAny<IEnumerable<string>>())
            )
            .Returns(expectedToken);
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockCartRepository.Setup(r => r.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<AuthResult>();
        result.Value.Id.Should().Be(user.Id.Value);
        result.Value.FirstName.Should().Be(user.FirstName);
        result.Value.LastName.Should().Be(user.LastName);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Token.Should().Be(expectedToken);

        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    It.Is<List<string>>(roles => roles.Count == 0),
                    It.Is<IEnumerable<string>>(perms =>
                        perms.SequenceEqual(new[] { permissions[0].Name, permissions[1].Name })
                    )
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockCartRepository.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
