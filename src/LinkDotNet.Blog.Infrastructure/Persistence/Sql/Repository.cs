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
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    public Repository(IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<TEntity> GetByIdAsync(string id)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        return await blogDbContext.Set<TEntity>().SingleOrDefaultAsync(b => b.Id == id);
    }

    public async ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var entity = blogDbContext.Set<TEntity>().AsNoTracking().AsQueryable();

        if (filter != null)
        {
            entity = entity.Where(filter);
        }

        if (orderBy != null)
        {
            entity = descending
                ? entity.OrderByDescending(orderBy)
                : entity.OrderBy(orderBy);
        }

        return await entity.ToPagedListAsync(page, pageSize);
    }

    public async ValueTask StoreAsync(TEntity entity)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
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

    public async ValueTask DeleteAsync(string id)
    {
        await using var blogDbContext = await dbContextFactory.CreateDbContextAsync();
        var entityToDelete = await GetByIdAsync(id);
        if (entityToDelete != null)
        {
            blogDbContext.Remove(entityToDelete);
            await blogDbContext.SaveChangesAsync();
        }
    }
}