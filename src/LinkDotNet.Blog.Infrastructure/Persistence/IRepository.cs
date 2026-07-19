using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    ValueTask<HealthCheckResult> PerformHealthCheckAsync();

    ValueTask<TEntity?> GetByIdAsync(string id);

    ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> selector,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IReadOnlyList<TResult>> GetGroupedByAsync<TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<IGrouping<TKey, TEntity>, TResult>> resultSelector,
        Expression<Func<TEntity, bool>>? filter = null);

    ValueTask StoreAsync(TEntity entity);

    ValueTask DeleteAsync(string id);

    ValueTask DeleteBulkAsync(IReadOnlyCollection<string> ids);

    ValueTask StoreBulkAsync(IReadOnlyCollection<TEntity> records);
}
