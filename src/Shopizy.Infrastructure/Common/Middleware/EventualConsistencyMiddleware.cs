using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate Next, ILogger<EventualConsistencyMiddleware> logger)
{
    private readonly ILogger<EventualConsistencyMiddleware> _logger = logger;
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, AppDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(publisher);
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
                        await publisher.Publish(nextEvent);
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
            return;
        }
    }
}
