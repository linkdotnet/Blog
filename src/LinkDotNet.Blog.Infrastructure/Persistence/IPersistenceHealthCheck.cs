using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IPersistenceHealthCheck
{
    ValueTask<HealthCheckResult> PerformHealthCheckAsync();
}
