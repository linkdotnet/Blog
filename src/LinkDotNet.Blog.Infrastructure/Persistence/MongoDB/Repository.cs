using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.MongoDB;

public sealed class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly IMongoDatabase database;
    private IMongoCollection<TEntity> Collection => database.GetCollection<TEntity>(typeof(TEntity).Name);

    public Repository(IMongoDatabase database)
    {
        this.database = database;
    }

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

    public async ValueTask<TEntity> GetByIdAsync(string id)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        var result = await Collection.FindAsync(filter);
        return await result.FirstOrDefaultAsync();
    }

    public async ValueTask<IPagedList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await GetAllByProjectionAsync(s => s, filter, orderBy, descending, page, pageSize);

    public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>>? selector,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        var query = Collection.AsQueryable();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (orderBy is not null)
        {
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }

        var projectionQuery = query.Select(selector);
        return await projectionQuery.ToPagedListAsync(page, pageSize);
    }

    public async ValueTask StoreAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
        }

        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, entity.Id);
        var options = new ReplaceOptions { IsUpsert = true };
        await Collection.ReplaceOneAsync(filter, entity, options);
    }

    public async ValueTask DeleteAsync(string id)
    {
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, id);
        await Collection.DeleteOneAsync(filter);
    }

    public async ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids)
    {
        var filter = Builders<TEntity>.Filter.In(doc => doc.Id, ids);
        await Collection.DeleteManyAsync(filter);
    }

    public async ValueTask StoreBulkAsync(IReadOnlyCollection<TEntity> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        if (records.Count != 0)
        {
            await Collection.InsertManyAsync(records);
        }
    }
}
