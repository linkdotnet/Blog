using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CachedRepository<T> : IRepository<T>
    where T : Entity
{
    private readonly IRepository<T> repository;
    private readonly IFusionCache fusionCache;

    public CachedRepository(IRepository<T> repository, IFusionCache fusionCache)
    {
        this.repository = repository;
        this.fusionCache = fusionCache;
    }

    public ValueTask<HealthCheckResult> PerformHealthCheckAsync() => repository.PerformHealthCheckAsync();

    public async ValueTask<T?> GetByIdAsync(string id) => await fusionCache.GetOrSetAsync(id, async c =>
        {
            return await repository.GetByIdAsync(id);
        }, TimeSpan.FromDays(7));

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
            await fusionCache.RemoveAsync(entity.Id);
        }
    }

    public async ValueTask DeleteAsync(string id)
    {
        await repository.DeleteAsync(id);
        await fusionCache.RemoveAsync(id);
    }

    public async ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids) => await repository.DeleteBulkAsync(ids);

    public async ValueTask StoreBulkAsync(IReadOnlyCollection<T> records) => await repository.StoreBulkAsync(records);
}
