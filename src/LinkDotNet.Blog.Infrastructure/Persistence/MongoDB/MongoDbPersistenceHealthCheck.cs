using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public sealed class MongoDbPersistenceHealthCheck(IMongoDatabase database) : IPersistenceHealthCheck
{
    public async ValueTask<HealthCheckResult> PerformHealthCheckAsync()
    {
        try
        {
            var command = new BsonDocument("ping", 1);
            await database.RunCommandAsync<BsonDocument>(command);

            return HealthCheckResult.Healthy("A healthy result.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
