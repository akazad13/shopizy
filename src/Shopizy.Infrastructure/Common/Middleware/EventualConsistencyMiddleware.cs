using MediatR;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Common.Models;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate _next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(HttpContext context, IPublisher publisher, AppDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) && value is Queue<IDomainEvent> domainEvent)
                {
                    while (domainEvent.TryDequeue(out var nextEvent))
                    {
                        await publisher.Publish(nextEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Ignore for now
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });

        await _next(context);
    }
}