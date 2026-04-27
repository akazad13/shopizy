using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Authentication;

namespace Shopizy.Infrastructure.Security.RefreshTokens;

public sealed class RefreshTokenGenerator(IOptions<RefreshTokenSettings> options) : IRefreshTokenGenerator
{
    private readonly RefreshTokenSettings _settings = options.Value;

    public TimeSpan Lifetime => TimeSpan.FromDays(_settings.ExpirationDays);

    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }
}
