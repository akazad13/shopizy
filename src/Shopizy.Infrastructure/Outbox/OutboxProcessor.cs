using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Outbox;

/// <summary>
/// Background service that retries domain events that were persisted to the outbox
/// but never successfully dispatched (e.g., because the process crashed after the
/// transaction committed but before the in-process handler ran).
/// </summary>
public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger
) : BackgroundService
{
    private static readonly TimeSpan s_pollingInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan s_processingThreshold = TimeSpan.FromMinutes(5);
    private const int DeadLetterBacklogThreshold = 10;

    // LoggerMessage delegates
    private static readonly Action<ILogger, int, Exception?> s_processingMessages =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(1, nameof(OutboxProcessor)),
            "Outbox: processing {Count} pending message(s).");

    private static readonly Action<ILogger, Guid, string, Exception?> s_deadLetteringMessageType =
        LoggerMessage.Define<Guid, string>(
            LogLevel.Warning,
            new EventId(2, nameof(OutboxProcessor)),
            "Outbox: dead-lettering message {Id} — {Reason}");

    private static readonly Action<ILogger, Guid, string, Exception?> s_deadLetteringMessageDeserialize =
        LoggerMessage.Define<Guid, string>(
            LogLevel.Warning,
            new EventId(3, nameof(OutboxProcessor)),
            "Outbox: dead-lettering message {Id} — {Reason}");

    private static readonly Action<ILogger, Guid, string, Exception?> s_processedMessage =
        LoggerMessage.Define<Guid, string>(
            LogLevel.Information,
            new EventId(4, nameof(OutboxProcessor)),
            "Outbox: processed message {Id} ({Type}).");

    private static readonly Action<ILogger, Guid, string, Exception?> s_failedToProcessMessage =
        LoggerMessage.Define<Guid, string>(
            LogLevel.Error,
            new EventId(5, nameof(OutboxProcessor)),
            "Outbox: failed to process message {Id} ({Type}).");

    private static readonly Action<ILogger, Exception?> s_unhandledError =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(6, nameof(OutboxProcessor)),
            "Outbox processor encountered an unhandled error.");

    private static readonly Action<ILogger, int, int, Exception?> s_deadLetterBacklog =
        LoggerMessage.Define<int, int>(
            LogLevel.Warning,
            new EventId(7, nameof(OutboxProcessor)),
            "Outbox dead-letter backlog has reached {Count} (threshold {Threshold}). Manual review required.");

    private static readonly System.Diagnostics.Metrics.Meter s_meter = new("Shopizy.Outbox", "1.0");
    private static readonly System.Diagnostics.Metrics.Counter<long> s_deadLettered =
        s_meter.CreateCounter<long>("shopizy.outbox.dead_lettered", unit: "{messages}", description: "Number of outbox messages newly dead-lettered.");
    private static readonly System.Diagnostics.Metrics.Counter<long> s_processed =
        s_meter.CreateCounter<long>("shopizy.outbox.processed", unit: "{messages}", description: "Number of outbox messages successfully processed.");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await processPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                s_unhandledError(logger, ex);
            }

            await Task.Delay(s_pollingInterval, stoppingToken);
        }
    }

    private async Task processPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        var threshold = DateTime.UtcNow - s_processingThreshold;

        var deadLetterCount = await dbContext.OutboxMessages
            .CountAsync(m => m.DeadLetteredOn != null, cancellationToken);
        if (deadLetterCount >= DeadLetterBacklogThreshold)
        {
            s_deadLetterBacklog(logger, deadLetterCount, DeadLetterBacklogThreshold, null);
        }

        var pending = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.DeadLetteredOn == null && m.OccurredOn <= threshold)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
        {
            return;
        }

        s_processingMessages(logger, pending.Count, null);

        foreach (var message in pending)
        {
            try
            {
                var eventType = System.Type.GetType(message.Type);
                if (eventType is null)
                {
                    var reason = $"Cannot resolve type '{message.Type}'.";
                    s_deadLetteringMessageType(logger, message.Id, reason, null);
                    await dbContext.OutboxMessages
                        .Where(m => m.Id == message.Id)
                        .ExecuteUpdateAsync(
                            s => s.SetProperty(p => p.DeadLetteredOn, DateTime.UtcNow)
                                  .SetProperty(p => p.DeadLetterReason, reason),
                            cancellationToken);
                    s_deadLettered.Add(1);
                    continue;
                }

                if (JsonSerializer.Deserialize(message.Content, eventType) is not IDomainEvent domainEvent)
                {
                    var reason = $"Cannot deserialize content as '{eventType.Name}'.";
                    s_deadLetteringMessageDeserialize(logger, message.Id, reason, null);
                    await dbContext.OutboxMessages
                        .Where(m => m.Id == message.Id)
                        .ExecuteUpdateAsync(
                            s => s.SetProperty(p => p.DeadLetteredOn, DateTime.UtcNow)
                                  .SetProperty(p => p.DeadLetterReason, reason),
                            cancellationToken);
                    s_deadLettered.Add(1);
                    continue;
                }

                await dispatcher.PublishAsync(domainEvent, cancellationToken);

                await dbContext.OutboxMessages
                    .Where(m => m.Id == message.Id)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.ProcessedOn, DateTime.UtcNow),
                        cancellationToken);

                s_processed.Add(1);
                s_processedMessage(logger, message.Id, eventType.Name, null);
            }
            catch (Exception ex)
            {
                s_failedToProcessMessage(logger, message.Id, message.Type, ex);
            }
        }
    }
}
