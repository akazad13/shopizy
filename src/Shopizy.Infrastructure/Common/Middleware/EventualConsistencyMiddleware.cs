using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Common.Middleware;

/// <summary>
/// Wraps each mutation in a database transaction and dispatches collected domain events
/// <b>inside</b> the same transaction. This makes the original aggregate write and any handler
/// side-effects atomic — if a handler throws, the request fails and the transaction rolls back,
/// preventing phantom orders / unmatched stock / cleared-but-not-checked-out carts.
/// <para>
/// <b>Handler contract:</b> see <c>docs/EventualConsistency.md</c>. Handlers must be DB-only
/// (no out-of-band side effects without the outbox) and may be retried by the EF execution strategy.
/// </para>
/// </summary>
/// <param name="Next"></param>
/// <param name="logger"></param>
public class EventualConsistencyMiddleware(
    RequestDelegate Next,
    ILogger<EventualConsistencyMiddleware> logger
)
{
    private readonly ILogger<EventualConsistencyMiddleware> _logger = logger;
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(
        HttpContext context,
        IDispatcher dispatcher,
        AppDbContext dbContext
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(dbContext);

        if (!HttpMethods.IsGet(method: context.Request.Method))
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
                    await dbContext.Database.BeginTransactionAsync();

                await Next(context);

                // Dispatch domain events INSIDE the transaction so handler side-effects
                // commit atomically with the original aggregate change. A handler that throws
                // aborts the whole request; OutboxMessages roll back with the rest.
                await DispatchPendingEventsAsync(context, dispatcher, dbContext);

                await transaction.CommitAsync();
                await transaction.DisposeAsync();
            });
        }
        else
        {
            await Next(context);
        }
    }

    private async Task DispatchPendingEventsAsync(
        HttpContext context,
        IDispatcher dispatcher,
        AppDbContext dbContext
    )
    {
        if (
            !context.Items.TryGetValue(DomainEventsKey, out object? value)
            || value is not Queue<(IDomainEvent Event, Guid OutboxId)> domainEventsQueue
        )
        {
            return;
        }

        // Handlers may call SaveChangesAsync, which can enqueue further events. Drain until empty.
        while (domainEventsQueue.TryDequeue(out var entry))
        {
            var (nextEvent, outboxId) = entry;

            try
            {
                await dispatcher.PublishAsync(nextEvent);
            }
            catch (Exception ex)
            {
                // Log and rethrow — failing inside the transaction rolls back the whole unit of work,
                // so the caller sees a 500 and there is no phantom write. Dead-letter alerting is
                // surfaced by OutboxProcessor for any pre-existing rows that survive a crash.
                var eventType = nextEvent.GetType().Name;
                var payload = JsonSerializer.Serialize(nextEvent, nextEvent.GetType());
                _logger.DomainEventPublishingError(ex);
                _logger.DomainEventDeadLettered(eventType, payload);
                throw;
            }

            await dbContext
                .OutboxMessages.Where(m => m.Id == outboxId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ProcessedOn, DateTime.UtcNow));
        }
    }
}
