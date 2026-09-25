using LinkDotNet.Blog.Domain;
using MongoDB.Driver;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

internal static class MongoDatabaseExtensions
{
    public static IMongoCollection<TEntity> CollectionFor<TEntity>(this IMongoDatabase database)
        where TEntity : Entity
        => database.GetCollection<TEntity>(typeof(TEntity).Name);
}
