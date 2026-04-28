namespace Shopizy.Application.Common.Interfaces.Services;

public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> TryGetAsync(string key, CancellationToken cancellationToken = default);

    Task StoreAsync(
        string key,
        IdempotencyRecord record,
        TimeSpan ttl,
        CancellationToken cancellationToken = default
    );
}

public sealed record IdempotencyRecord(
    string RequestHash,
    int StatusCode,
    string ContentType,
    byte[] Body
);
