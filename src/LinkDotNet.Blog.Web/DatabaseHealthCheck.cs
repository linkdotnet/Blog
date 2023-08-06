using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Web;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IRepository<BlogPost> repository;

    public DatabaseHealthCheck(IRepository<BlogPost> repository)
    {
        this.repository = repository;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken =  default) =>
        repository.PerformHealthCheckAsync().AsTask();
}
