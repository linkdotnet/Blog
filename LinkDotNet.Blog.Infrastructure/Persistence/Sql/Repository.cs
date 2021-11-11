using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly BlogDbContext blogDbContext;

    public Repository(BlogDbContext blogDbContext)
    {
        this.blogDbContext = blogDbContext;
    }

    public async Task<TEntity> GetByIdAsync(string id)
    {
        return await blogDbContext.Set<TEntity>().SingleOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        var entity = blogDbContext.Set<TEntity>().AsNoTracking().AsQueryable();

        if (filter != null)
        {
            entity = entity.Where(filter);
        }

        if (orderBy != null)
        {
            if (descending)
            {
                return await entity.OrderByDescending(orderBy).ToPagedListAsync(page, pageSize);
            }

            return await entity.OrderBy(orderBy).ToPagedListAsync(page, pageSize);
        }

        return await entity.ToPagedListAsync(page, pageSize);
    }

    public async Task StoreAsync(TEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            await blogDbContext.Set<TEntity>().AddAsync(entity);
        }
        else
        {
            blogDbContext.Entry(entity).State = EntityState.Modified;
        }

        await blogDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var entityToDelete = await GetByIdAsync(id);
        if (entityToDelete != null)
        {
            blogDbContext.Remove(entityToDelete);
            await blogDbContext.SaveChangesAsync();
        }
    }
}
