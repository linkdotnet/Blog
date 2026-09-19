using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class SqlPersistenceHealthCheck(IDbContextFactory<BlogDbContext> dbContextFactory) : IPersistenceHealthCheck
{
    public async ValueTask<HealthCheckResult> PerformHealthCheckAsync()
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync();
            await db.Database.ExecuteSqlRawAsync("SELECT 1");
            return HealthCheckResult.Healthy();
        }
        catch (Exception exc)
        {
            return HealthCheckResult.Unhealthy(exception: exc);
        }
    }
}
