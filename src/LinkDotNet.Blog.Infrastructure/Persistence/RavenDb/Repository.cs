using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly IDocumentStore documentStore;

    public Repository(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async ValueTask<TEntity> GetByIdAsync(string id)
    {
        using var session = documentStore.OpenAsyncSession();
        return await session.LoadAsync<TEntity>(id);
    }

    public async ValueTask<IPaginatedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        return await GetAllByProjectionAsync(s => s, filter, orderBy, descending, page, pageSize);
    }

    public async ValueTask<IPaginatedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> selector,
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(selector);
        using var session = documentStore.OpenAsyncSession();

        var query = session.Query<TEntity>();
        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        return await query.Select(selector).ToPagedListAsync(page, pageSize);
    }

    public async ValueTask StoreAsync(TEntity entity)
    {
        using var session = documentStore.OpenAsyncSession();
        await session.StoreAsync(entity);
        await session.SaveChangesAsync();
    }

    public async ValueTask DeleteAsync(string id)
    {
        using var session = documentStore.OpenAsyncSession();
        session.Delete(id);
        await session.SaveChangesAsync();
    }
}
