namespace Shopizy.Infrastructure.Common.Caching;

public class RedisSettings
{
    public const string Section = "RedisCacheSettings";
    public string Endpoint { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
