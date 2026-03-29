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
using Shopizy.Domain.Users.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.Application.UnitTests.Auth.Queries.login;

public class LoginQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPermissionRepository> _mockPermissionRepository;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IPasswordManager> _mockPasswordManager;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordManager = new Mock<IPasswordManager>();
        _mockCartRepository = new Mock<ICartRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new LoginQueryHandler(
            _mockUserRepository.Object,
            _mockPermissionRepository.Object,
            _mockJwtTokenGenerator.Object,
            _mockPasswordManager.Object,
            _mockCartRepository.Object,
            _mockUnitOfWork.Object
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
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.User.UserNotFoundWhileLogin, result.FirstError);
    }

    [Fact]
    public async Task ShouldReturnsInvalidCredentialsErrorWhenInvalidPassword()
    {
        // Arrange
        var query = new LoginQuery("test@test.com", "password");
        var user = User.Create("John", "Doe", "test@test.com", "password", UserRole.Customer, []);

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password)).Returns(false);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Authentication.InvalidCredentials, result.FirstError);
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
                    user.Role.ToString(),
                    It.Is<IEnumerable<string>>(p =>
                        p.SequenceEqual(new[] { permissions[0].Name, permissions[1].Name })
                    )
                )
            )
            .Returns(expectedToken);
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(
            new AuthResult(
                user.Id.Value,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.ToString(),
                expectedToken
            ),
            result.Value
        );
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    user.Role.ToString(),
                    It.Is<IEnumerable<string>>(p =>
                        p.SequenceEqual(new[] { permissions[0].Name, permissions[1].Name })
                    )
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnsAuthResultWithEmptyPermissions_WhenEmptyPermissionList()
    {
        // Arrange
        var query = LoginQueryUtils.CreateQuery();
        var user = UserFactory.CreateUser();

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockPasswordManager.Setup(p => p.Verify(query.Password, user.Password!)).Returns(true);
        _mockPermissionRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<Permission>().AsReadOnly());
        _mockJwtTokenGenerator
            .Setup(j =>
                j.GenerateToken(user.Id, user.Role.ToString(), It.IsAny<IEnumerable<string>>())
            )
            .Returns("generatedToken");
        _mockCartRepository.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<AuthResult>(result.Value);
        Assert.Equal(user.Id.Value, result.Value.Id);
        Assert.Equal(user.FirstName, result.Value.FirstName);
        Assert.Equal(user.LastName, result.Value.LastName);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.Role.ToString(), result.Value.Role);
        Assert.Equal("generatedToken", result.Value.Token);
        _mockJwtTokenGenerator.Verify(
            j =>
                j.GenerateToken(
                    user.Id,
                    user.Role.ToString(),
                    It.Is<IEnumerable<string>>(p => !p.Any())
                ),
            Times.Once
        );
        _mockCartRepository.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
}
