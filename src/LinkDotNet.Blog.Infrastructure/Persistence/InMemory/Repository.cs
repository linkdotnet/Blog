using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence.InMemory;

public sealed class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly List<TEntity> entities = [];

    public ValueTask<HealthCheckResult> PerformHealthCheckAsync() => ValueTask.FromResult(HealthCheckResult.Healthy());

    public ValueTask<TEntity> GetByIdAsync(string id)
    {
        var entity = entities.SingleOrDefault(b => b.Id == id);
        return new ValueTask<TEntity>(entity);
    }

    public ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        GetAllByProjectionAsync(s => s, filter, orderBy, descending, page, pageSize);

    public ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> selector,
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var result = entities.AsEnumerable();
        if (filter != null)
        {
            result = result.Where(filter.Compile());
        }

        if (orderBy != null)
        {
            result = descending
                ? result.OrderByDescending(orderBy.Compile())
                : result.OrderBy(orderBy.Compile());
        }

        return new ValueTask<IPagedList<TProjection>>(result.Select(selector.Compile()).ToPagedList(page, pageSize));
    }

    public ValueTask StoreAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = entities.Max(b => b.Id) + 1;
        }

        var entry = entities.SingleOrDefault(b => b.Id == entity.Id);
        if (entry != null)
        {
            entities.Remove(entry);
        }

        entities.Add(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(string id)
    {
        var blogPostToDelete = entities.SingleOrDefault(b => b.Id == id);
        if (blogPostToDelete != null)
        {
            entities.Remove(blogPostToDelete);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        foreach (var id in ids)
        {
            var entity = entities.First(e => e.Id == id);
            entities.Remove(entity);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask StoreBulkAsync(IReadOnlyCollection<TEntity> records)
    {
        entities.AddRange(records);
        return ValueTask.CompletedTask;
    }
}
