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

    public Task<TEntity> GetByIdAsync(string id)
    {
        var entity = entities.SingleOrDefault(b => b.Id == id);
        return Task.FromResult(entity);
    }

    public Task<IPagedList<TEntity>> GetAllAsync(
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
            if (descending)
            {
                return Task.FromResult(result.OrderByDescending(orderBy.Compile()).ToPagedList(page, pageSize));
            }

            return Task.FromResult(result.OrderBy(orderBy.Compile()).ToPagedList(page, pageSize));
        }

        return Task.FromResult(entities.ToPagedList(page, pageSize));
    }

    public Task StoreAsync(TEntity entity)
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
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        var blogPostToDelete = entities.SingleOrDefault(b => b.Id == id);
        if (blogPostToDelete != null)
        {
            entities.Remove(blogPostToDelete);
        }

        return Task.CompletedTask;
    }
}
