namespace Shopizy.Application.Common.Caching;

public class RedisSettings
{
    public const string Section = "RedisCacheSettings";
    public string ConnectionString { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
