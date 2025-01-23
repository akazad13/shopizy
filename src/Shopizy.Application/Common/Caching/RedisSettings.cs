namespace Shopizy.Application.Common.Caching;

public class RedisSettings
{
    public const string Section = "RedisCacheSettings";
    public string Endpoint { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
