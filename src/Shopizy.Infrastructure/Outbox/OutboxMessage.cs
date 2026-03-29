namespace Shopizy.Infrastructure.Outbox;

/// <summary>
/// Persisted record of a domain event that must be reliably dispatched.
/// Written to the database in the same transaction as the aggregate change.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }

    /// <summary>Assembly-qualified type name used for deserialization.</summary>
    public string Type { get; init; } = null!;

    /// <summary>JSON-serialized event payload.</summary>
    public string Content { get; init; } = null!;

    /// <summary>Set once the event has been successfully dispatched.</summary>
    public DateTime? ProcessedOn { get; set; }

    /// <summary>Set when the message cannot be deserialized or its type cannot be resolved; prevents infinite retry loops.</summary>
    public DateTime? DeadLetteredOn { get; set; }

    /// <summary>Human-readable reason the message was dead-lettered.</summary>
    public string? DeadLetterReason { get; set; }
}
