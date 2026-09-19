using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Web;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IPersistenceHealthCheck repository;

    public DatabaseHealthCheck(IPersistenceHealthCheck repository)
    {
        this.repository = repository;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        repository.PerformHealthCheckAsync().AsTask();
}
