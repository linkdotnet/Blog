using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using X.PagedList;

namespace LinkDotNet.Infrastructure.Persistence
{
    public interface IRepository<TEntity>
    where TEntity : Entity
    {
        Task<TEntity> GetByIdAsync(string id, Expression<Func<TEntity, object>> include = null);

        Task<IPagedList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity,
                object>> orderBy = null,
            Expression<Func<TEntity, object>> include = null,
            bool descending = true,
            int page = 1,
            int pageSize = int.MaxValue);

        Task StoreAsync(TEntity entity);

        Task DeleteAsync(string id);
    }
}