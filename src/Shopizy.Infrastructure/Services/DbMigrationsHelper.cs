using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Services;

[ExcludeFromCodeCoverage]
public class DbMigrationsHelper(ILogger<DbMigrationsHelper> logger, AppDbContext context)
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<DbMigrationsHelper> _logger = logger;

    public async Task MigrateAsync()
    {
        try
        {
            if (
                _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer"
                && (await _context.Database.GetPendingMigrationsAsync()).Any()
            )
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.DatabaseInitializationError(ex);
            throw;
        }
    }
}
