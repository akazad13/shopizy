namespace Shopizy.Infrastructure.Outbox;

/// <summary>
/// Drains pending outbox messages on demand. The background <see cref="OutboxProcessor"/>
/// only retries messages older than its processing threshold; tests use this to flush
/// the outbox synchronously without sleeping.
/// </summary>
public interface IOutboxDrainer
{
    /// <summary>
    /// Processes every pending (unprocessed and not dead-lettered) outbox message immediately,
    /// regardless of age, and returns the number of messages successfully dispatched.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<int> DrainAsync(CancellationToken cancellationToken = default);
}
