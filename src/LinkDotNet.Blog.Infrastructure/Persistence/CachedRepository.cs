using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CachedRepository<T> : IRepository<T>
    where T : Entity
{
    private readonly IRepository<T> repository;
    private readonly IMemoryCache memoryCache;

    public CachedRepository(IRepository<T> repository, IMemoryCache memoryCache)
    {
        this.repository = repository;
        this.memoryCache = memoryCache;
    }

    public ValueTask<HealthCheckResult> PerformHealthCheckAsync() => repository.PerformHealthCheckAsync();

    public async ValueTask<T> GetByIdAsync(string id)
    {
        return await memoryCache.GetOrCreateAsync(id, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromDays(7);
            return await repository.GetByIdAsync(id);
        });
    }

    public async ValueTask<IPagedList<T>> GetAllAsync(
        Expression<Func<T, bool>> filter = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<T, TProjection>> selector,
        Expression<Func<T, bool>> filter = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllByProjectionAsync(selector, filter, orderBy, descending, page, pageSize);

    public async ValueTask StoreAsync(T entity)
    {
        await repository.StoreAsync(entity);

        if (!string.IsNullOrEmpty(entity.Id) && memoryCache.TryGetValue(entity.Id, out _))
        {
            memoryCache.Remove(entity.Id);
        }
    }

    public async ValueTask DeleteAsync(string id)
    {
        await repository.DeleteAsync(id);

        if (memoryCache.TryGetValue(id, out _))
        {
            memoryCache.Remove(id);
        }
    }

    public async ValueTask DeleteBulkAsync(IEnumerable<string> ids) => await repository.DeleteBulkAsync(ids);

    public async ValueTask StoreBulkAsync(IEnumerable<T> records) => await repository.StoreBulkAsync(records);
}
