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
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Only retry messages older than this threshold, giving the synchronous
    /// in-process handler (EventualConsistencyMiddleware) time to mark them processed.
    /// </summary>
    private static readonly TimeSpan ProcessingThreshold = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox processor encountered an unhandled error.");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        var threshold = DateTime.UtcNow - ProcessingThreshold;

        var pending = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.DeadLetteredOn == null && m.OccurredOn <= threshold)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
        {
            return;
        }

        logger.LogInformation("Outbox: processing {Count} pending message(s).", pending.Count);

        foreach (var message in pending)
        {
            try
            {
                var eventType = System.Type.GetType(message.Type);
                if (eventType is null)
                {
                    var reason = $"Cannot resolve type '{message.Type}'.";
                    logger.LogWarning("Outbox: dead-lettering message {Id} — {Reason}", message.Id, reason);
                    await dbContext.OutboxMessages
                        .Where(m => m.Id == message.Id)
                        .ExecuteUpdateAsync(
                            s => s.SetProperty(p => p.DeadLetteredOn, DateTime.UtcNow)
                                  .SetProperty(p => p.DeadLetterReason, reason),
                            cancellationToken);
                    continue;
                }

                if (JsonSerializer.Deserialize(message.Content, eventType) is not IDomainEvent domainEvent)
                {
                    var reason = $"Cannot deserialize content as '{eventType.Name}'.";
                    logger.LogWarning("Outbox: dead-lettering message {Id} — {Reason}", message.Id, reason);
                    await dbContext.OutboxMessages
                        .Where(m => m.Id == message.Id)
                        .ExecuteUpdateAsync(
                            s => s.SetProperty(p => p.DeadLetteredOn, DateTime.UtcNow)
                                  .SetProperty(p => p.DeadLetterReason, reason),
                            cancellationToken);
                    continue;
                }

                await dispatcher.PublishAsync(domainEvent, cancellationToken);

                // Mark processed using ExecuteUpdateAsync (bypasses SaveChangesAsync override)
                await dbContext.OutboxMessages
                    .Where(m => m.Id == message.Id)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(p => p.ProcessedOn, DateTime.UtcNow),
                        cancellationToken);

                logger.LogInformation("Outbox: processed message {Id} ({Type}).", message.Id, eventType.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox: failed to process message {Id} ({Type}).", message.Id, message.Type);
            }
        }
    }
}
