using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Services;

public class DbMigrationsHelper(
    ILogger<DbMigrationsHelper> logger,
    AppDbContext context
    )
{
    public async Task MigrateAsync()
    {
        try
        {
            if (context.Database.IsSqlServer() && (await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}