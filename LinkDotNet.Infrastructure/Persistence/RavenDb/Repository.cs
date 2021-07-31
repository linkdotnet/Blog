using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using X.PagedList;

namespace LinkDotNet.Infrastructure.Persistence.RavenDb
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        private readonly IDocumentStore documentStore;

        public Repository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<TEntity> GetByIdAsync(string id, Expression<Func<TEntity, object>> include = null)
        {
            using var session = documentStore.OpenAsyncSession();
            return await session.LoadAsync<TEntity>(id);
        }

        public async Task<IPagedList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, object>> orderBy = null,
            Expression<Func<TEntity, object>> include = null,
            bool descending = true,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            using var session = documentStore.OpenAsyncSession();

            var query = session.Query<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                if (descending)
                {
                    return await query.OrderByDescending(orderBy).ToPagedListAsync(page, pageSize);
                }

                return await query.OrderBy(orderBy).ToPagedListAsync(page, pageSize);
            }

            return await query.ToPagedListAsync(page, pageSize);
        }

        public async Task StoreAsync(TEntity entity)
        {
            using var session = documentStore.OpenAsyncSession();
            await session.StoreAsync(entity);
            await session.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            using var session = documentStore.OpenAsyncSession();
            session.Delete(id);
            await session.SaveChangesAsync();
        }
    }
}