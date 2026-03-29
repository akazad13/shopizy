using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Outbox;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate Next, ILogger<EventualConsistencyMiddleware> logger)
{
    private readonly ILogger<EventualConsistencyMiddleware> _logger = logger;
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(HttpContext context, IDispatcher dispatcher, AppDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(dbContext);

        if (!HttpMethods.IsGet(method: context.Request.Method))
        {
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
                await dbContext.Database.BeginTransactionAsync();

            await Next(context);

            await transaction.CommitAsync();

            try
            {
                if (
                    context.Items.TryGetValue(DomainEventsKey, out object? value)
                    && value is Queue<(IDomainEvent Event, Guid OutboxId)> domainEventsQueue
                )
                {
                    while (domainEventsQueue.TryDequeue(out var entry))
                    {
                        var (nextEvent, outboxId) = entry;
                        var published = false;

                        for (var attempt = 0; attempt < 3 && !published; attempt++)
                        {
                            try
                            {
                                await dispatcher.PublishAsync(nextEvent);
                                published = true;
                            }
                            catch (Exception ex) when (attempt < 2)
                            {
                                _logger.DomainEventPublishingError(ex);
                                await Task.Delay(TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100));
                            }
                        }

                        if (published)
                        {
                            // Mark outbox message as processed so the background worker skips it
                            await dbContext.OutboxMessages
                                .Where(m => m.Id == outboxId)
                                .ExecuteUpdateAsync(
                                    s => s.SetProperty(p => p.ProcessedOn, DateTime.UtcNow));
                        }
                        else
                        {
                            var eventType = nextEvent.GetType().Name;
                            var payload = JsonSerializer.Serialize(nextEvent, nextEvent.GetType());
                            _logger.DomainEventDeadLettered(eventType, payload);
                            // OutboxMessage.ProcessedOn stays null → OutboxProcessor will retry
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.DomainEventPublishingError(ex);
                // If publishing fails, data is already committed.
                // OutboxProcessor will retry any unprocessed messages.
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
        else
        {
            await Next(context);
        }
    }
}
