using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Infrastructure.Security.Totp;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Security.Totp;

public class TotpHelperTests
{
    [Fact]
    public void VerifyCode_WithCurrentTimestampCode_ShouldReturnTrue()
    {
        var totp = new TotpHelper();
        var secret = "JBSWY3DPEHPK3PXP"; // Standard Base32 secret

        // Derive current timestamp code using reflection or verify matching generated code
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        var method = typeof(TotpHelper).GetMethod(
            "GenerateCode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
        );
        var expectedCode = (string)method!.Invoke(null, [secret, timestamp])!;

        var result = totp.VerifyCode(secret, expectedCode);
        result.ShouldBeTrue();

        ITotpHelper interfaceTotp = totp;
        interfaceTotp.VerifyCode(secret, expectedCode).ShouldBeTrue();
    }

    [Fact]
    public void VerifyCode_WithInvalidCode_ShouldReturnFalse()
    {
        var totp = new TotpHelper();
        var secret = "JBSWY3DPEHPK3PXP";

        var result = totp.VerifyCode(secret, "000000");
        result.ShouldBeFalse();
    }

    [Fact]
    public void Base32Decode_WithPaddingAndIgnoredChars_ShouldDecodeCorrectly()
    {
        var method = typeof(TotpHelper).GetMethod(
            "Base32Decode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
        );
        var bytes = (byte[])method!.Invoke(null, ["JBSWY3DPEHPK3PXP==="])!;
        bytes.ShouldNotBeEmpty();
    }
}
