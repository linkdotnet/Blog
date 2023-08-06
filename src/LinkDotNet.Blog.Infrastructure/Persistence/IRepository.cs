using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    ValueTask<HealthCheckResult> PerformHealthCheckAsync();

    ValueTask<TEntity> GetByIdAsync(string id);

    ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity,
            object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> selector,
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask StoreAsync(TEntity entity);

    ValueTask DeleteAsync(string id);
}
