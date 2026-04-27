using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.Security.RefreshTokens;

[ExcludeFromCodeCoverage]
public class RefreshTokenSettings
{
    public const string Section = "RefreshTokenSettings";
    public int ExpirationDays { get; set; } = 30;
}
