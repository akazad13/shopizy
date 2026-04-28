using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Outbox;

/// <summary>
/// Shared implementation used by both <see cref="OutboxProcessor"/> (background) and
/// integration tests. Processes every pending outbox message that is not yet dead-lettered.
/// </summary>
public sealed class OutboxDrainer(
    AppDbContext dbContext,
    IDispatcher dispatcher,
    ILogger<OutboxDrainer> logger
) : IOutboxDrainer
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IDispatcher _dispatcher = dispatcher;
    private readonly ILogger<OutboxDrainer> _logger = logger;

    private static readonly Action<ILogger, Guid, string, Exception?> _outboxDrainFailed = LoggerMessage.Define<Guid, string>(
        LogLevel.Error,
        new EventId(1, nameof(OutboxDrainer)),
        "Outbox drain: failed to process message {MessageId} ({Type}).");

    public async Task<int> DrainAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.DeadLetteredOn == null)
            .OrderBy(m => m.OccurredOn)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var processed = 0;
        foreach (var message in pending)
        {
            try
            {
                var eventType = Type.GetType(message.Type);
                if (eventType is null)
                {
                    await MarkDeadLetterAsync(message.Id, $"Cannot resolve type '{message.Type}'.", cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                if (JsonSerializer.Deserialize(message.Content, eventType) is not IDomainEvent domainEvent)
                {
                    await MarkDeadLetterAsync(message.Id, $"Cannot deserialize content as '{eventType.Name}'.", cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                await _dispatcher.PublishAsync(domainEvent, cancellationToken).ConfigureAwait(false);

                await _dbContext.OutboxMessages
                    .Where(m => m.Id == message.Id)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.ProcessedOn, DateTime.UtcNow),
                        cancellationToken)
                    .ConfigureAwait(false);

                processed++;
            }
#pragma warning disable CA1031 // outbox drain must keep going past a single bad message
            catch (Exception ex)
#pragma warning restore CA1031
            {
                _outboxDrainFailed(_logger, message.Id, message.Type, ex);
            }
        }

        return processed;
    }

    private Task MarkDeadLetterAsync(Guid id, string reason, CancellationToken cancellationToken)
    {
        return _dbContext.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.DeadLetteredOn, DateTime.UtcNow)
                      .SetProperty(p => p.DeadLetterReason, reason),
                cancellationToken);
    }
}
