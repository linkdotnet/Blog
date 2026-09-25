using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.MongoDB;

internal static class MongoDbTestContainer
{
    private static readonly TestContainer Container = TestContainer.Create("MongoDB", () => new MongoDbBuilder("mongo:7").Build(), c => c.GetConnectionString());

    public static async Task<IMongoDatabase> CreateDatabaseAsync() =>
        MongoDBConnectionProvider.Create(await Container.GetConnectionStringAsync(), Guid.NewGuid().ToString("N"));
}
