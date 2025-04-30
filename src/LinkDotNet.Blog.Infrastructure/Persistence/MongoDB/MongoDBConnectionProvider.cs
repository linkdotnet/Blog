using LinkDotNet.Blog.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public static class MongoDBConnectionProvider
{
    public static IMongoDatabase Create(string connectionString, string databaseName)
    {
#pragma warning disable IDISP001 // Handled by DI container
#pragma warning disable CA2000 // Handled by DI container
        var client = new MongoClient(connectionString);
#pragma warning restore CA2000
#pragma warning restore IDISP001
        BsonClassMap.RegisterClassMap<Entity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(e => e.Id);
        });
        return client.GetDatabase(databaseName);
    }
}
