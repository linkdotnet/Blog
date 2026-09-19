using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Raven.Client.Documents;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public sealed class RavenDbPersistenceHealthCheck(IDocumentStore documentStore) : IPersistenceHealthCheck
{
    public async ValueTask<HealthCheckResult> PerformHealthCheckAsync()
    {
        try
        {
            using var session = documentStore.OpenAsyncSession();
            await session.Query<object>().FirstOrDefaultAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}
