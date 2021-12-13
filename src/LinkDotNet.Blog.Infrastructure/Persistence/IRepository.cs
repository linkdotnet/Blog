using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using X.PagedList;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IRepository<TEntity>
where TEntity : Entity
{
    ValueTask<TEntity> GetByIdAsync(string id);

    ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity,
            object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask StoreAsync(TEntity entity);

    ValueTask DeleteAsync(string id);
}
