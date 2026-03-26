using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
                    && value is Queue<IDomainEvent> domainEvent
                )
                {
                    while (domainEvent.TryDequeue(out IDomainEvent? nextEvent))
                    {
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
                        if (!published)
                        {
                            _logger.DomainEventPublishingError(
                                new Exception($"Failed to publish {nextEvent.GetType().Name} after 3 attempts"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.DomainEventPublishingError(ex);
                // If publishing fails, data is already committed.
                // This is "Best Effort" consistency.
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
