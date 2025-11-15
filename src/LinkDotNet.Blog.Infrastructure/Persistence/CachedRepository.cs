using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AsyncKeyedLock;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CachedRepository<T> : IRepository<T>
    where T : Entity
{
    private readonly IRepository<T> repository;
    private readonly IMemoryCache memoryCache;
    private readonly AsyncKeyedLocker<string> asyncKeyedLocker;

    public CachedRepository(IRepository<T> repository, IMemoryCache memoryCache, AsyncKeyedLocker<string> asyncKeyedLocker)
    {
        this.repository = repository;
        this.memoryCache = memoryCache;
        this.asyncKeyedLocker = asyncKeyedLocker;
    }

    public ValueTask<HealthCheckResult> PerformHealthCheckAsync() => repository.PerformHealthCheckAsync();

    public async ValueTask<T?> GetByIdAsync(string id) => await GetByIdAsync(id, TimeSpan.FromDays(7));

    public async ValueTask<T?> GetByIdAsync(string id, TimeSpan slidingExpiration)
    {
        if (memoryCache.TryGetValue(id, out T? cachedObj))
        {
            return cachedObj;
        }

        using (await asyncKeyedLocker.LockAsync(id))
        {
            if (memoryCache.TryGetValue(id, out cachedObj))
            {
                return cachedObj;
            }

            var value = await repository.GetByIdAsync(id);

            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration
            };

            memoryCache.Set(id, value, options);

            return value;
        }
    }

    public async ValueTask<IPagedList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<T, TProjection>> selector,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        await repository.GetAllByProjectionAsync(selector, filter, orderBy, descending, page, pageSize);

    public async ValueTask StoreAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await repository.StoreAsync(entity);

        if (!string.IsNullOrEmpty(entity.Id))
        {
            memoryCache.Remove(entity.Id);
        }
    }

    public async ValueTask DeleteAsync(string id)
    {
        await repository.DeleteAsync(id);
        memoryCache.Remove(id);
    }

    public async ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids) => await repository.DeleteBulkAsync(ids);

    public async ValueTask StoreBulkAsync(IReadOnlyCollection<T> records) => await repository.StoreBulkAsync(records);
}
