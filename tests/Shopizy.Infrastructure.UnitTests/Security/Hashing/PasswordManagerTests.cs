using Shopizy.Infrastructure.Security.Hashing;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Security.Hashing;

public class PasswordManagerTests
{
    [Fact]
    public void Verify_WithInvalidBase64_ShouldReturnFalse()
    {
        var pm = new PasswordManager();
        var result = pm.Verify("password", "invalid_base64!!!");
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsLatestHastversion_StringAndBytes_ShouldReturnTrueForNewHash()
    {
        var pm = new PasswordManager();
        var hashStr = pm.CreateHashString("ValidPassword123");
        var hashBytes = Convert.FromBase64String(hashStr);

        pm.IsLatestHastversion(hashStr).ShouldBeTrue();
        pm.IsLatestHastversion(hashBytes).ShouldBeTrue();
    }
}
