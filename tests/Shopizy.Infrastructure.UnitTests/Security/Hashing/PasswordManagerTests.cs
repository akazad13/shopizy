using Xunit;
using Shopizy.Infrastructure.Security.Hashing;
using Shouldly;

namespace Shopizy.Infrastructure.UnitTests.Security.Hashing;

public class PasswordManagerTests
{
    private readonly PasswordManager _passwordManager;

    public PasswordManagerTests()
    {
        _passwordManager = new PasswordManager();
    }

    [Fact]
    public void CreateHashString_ShouldCreateBase64Hash()
    {
        // Arrange
        var password = "StrongPassword123!";
        var iterations = 10000;

        // Act
        var hash = _passwordManager.CreateHashString(password, iterations);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        bool isBase64 = IsBase64String(hash);
        isBase64.ShouldBeTrue();
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "MySecretPassword";
        var iterations = 5000;
        var hash = _passwordManager.CreateHashString(password, iterations);

        // Act
        var result = _passwordManager.Verify(password, hash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "MySecretPassword";
        var wrongPassword = "WrongPassword";
        var hash = _passwordManager.CreateHashString(password);

        // Act
        var result = _passwordManager.Verify(wrongPassword, hash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsLatestHastversion_ShouldReturnTrueForNewHash()
    {
        // Arrange
        var hash = _passwordManager.CreateHashString("test");

        // Act
        var result = _passwordManager.IsLatestHastversion(hash);

        // Assert
        result.ShouldBeTrue();
    }

    private bool IsBase64String(string base64)
    {
        Span<byte> buffer = new byte[base64.Length];
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
