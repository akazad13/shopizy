using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Common.Security.CurrentUser;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Security;

public class CurrentUserTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly CurrentUser _currentUser;

    public CurrentUserTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _currentUser = new CurrentUser(_mockHttpContextAccessor.Object);
    }

    [Fact]
    public void GetCurrentUserId_WhenHttpContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _currentUser.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WhenNoUserIdClaim_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
        };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => _currentUser.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WhenIdClaimPresent_ShouldReturnGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim("id", userId.ToString()) };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
        };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUser.GetCurrentUserId();

        // Assert
        result.ShouldBe(userId);
    }

    [Fact]
    public void GetCurrentUserId_WhenNameIdentifierClaimPresent_ShouldReturnGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
        };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUser.GetCurrentUserId();

        // Assert
        result.ShouldBe(userId);
    }

    [Fact]
    public void TryGetCurrentUserId_WhenHttpContextIsNull_ShouldReturnFalse()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        // Act
        var success = _currentUser.TryGetCurrentUserId(out var userId);

        // Assert
        success.ShouldBeFalse();
        userId.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void TryGetCurrentUserId_WhenClaimInvalidGuid_ShouldReturnFalse()
    {
        // Arrange
        var claims = new[] { new Claim("id", "not-a-guid") };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
        };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var success = _currentUser.TryGetCurrentUserId(out var userId);

        // Assert
        success.ShouldBeFalse();
        userId.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void TryGetCurrentUserId_WhenValidIdClaim_ShouldReturnTrueAndGuid()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var claims = new[] { new Claim("id", expectedId.ToString()) };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
        };
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var success = _currentUser.TryGetCurrentUserId(out var userId);

        // Assert
        success.ShouldBeTrue();
        userId.ShouldBe(expectedId);
    }
}
