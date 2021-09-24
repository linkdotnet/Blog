using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;

namespace LinkDotNet.Blog.Infrastructure.Persistence
{
    public class CachedRepository<T> : IRepository<T>
        where T : Entity
    {
        private readonly IRepository<T> repository;
        private readonly IMemoryCache memoryCache;

        public CachedRepository(IRepository<T> repository, IMemoryCache memoryCache)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            if (memoryCache.TryGetValue(id, out T item))
            {
                return item;
            }

            var fromDb = await repository.GetByIdAsync(id);
            memoryCache.Set(id, fromDb);
            return fromDb;
        }

        public async Task<IPagedList<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            var key = $"{page}-{pageSize}";
            return await memoryCache.GetOrCreate(key, async e =>
            {
                e.SetOptions(new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                });
                return await repository.GetAllAsync(filter, orderBy, descending, page, pageSize);
            });
        }

        public async Task StoreAsync(T entity)
        {
            await repository.StoreAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            await repository.DeleteAsync(id);
        }
    }
}