using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using X.PagedList;

namespace LinkDotNet.Blog.Infrastructure.Persistence.InMemory;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly List<TEntity> entities = new();

    public ValueTask<TEntity> GetByIdAsync(string id)
    {
        var entity = entities.SingleOrDefault(b => b.Id == id);
        return new ValueTask<TEntity>(entity);
    }

    public ValueTask<IPagedList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, object>> orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue)
    {
        var result = entities.AsEnumerable();
        if (filter != null)
        {
            result = result.Where(filter.Compile());
        }

        if (orderBy != null)
        {
            result = descending
                ? result.OrderByDescending(orderBy.Compile())
                : result.OrderBy(orderBy.Compile());
        }

        return new ValueTask<IPagedList<TEntity>>(result.ToPagedList(page, pageSize));
    }

    public ValueTask StoreAsync(TEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = entities.Max(b => b.Id) + 1;
        }

        var entry = entities.SingleOrDefault(b => b.Id == entity.Id);
        if (entry != null)
        {
            entities.Remove(entry);
        }

        entities.Add(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(string id)
    {
        var blogPostToDelete = entities.SingleOrDefault(b => b.Id == id);
        if (blogPostToDelete != null)
        {
            entities.Remove(blogPostToDelete);
        }

        return ValueTask.CompletedTask;
    }
}
