using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class CachedBlogPostRepository(IBlogPostRepository repository, IFusionCache fusionCache) : IBlogPostRepository
{
    public async ValueTask<BlogPost?> GetByIdAsync(string id) => await fusionCache.GetOrSetAsync(id, async _ =>
        await repository.GetByIdAsync(id), TimeSpan.FromDays(7));

    public ValueTask<IPagedList<BlogPost>> GetAllAsync(
        Expression<Func<BlogPost, bool>>? filter = null,
        Expression<Func<BlogPost, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        repository.GetAllAsync(filter, orderBy, descending, page, pageSize);

    public ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<BlogPost, TProjection>> selector,
        Expression<Func<BlogPost, bool>>? filter = null,
        Expression<Func<BlogPost, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue) =>
        repository.GetAllByProjectionAsync(selector, filter, orderBy, descending, page, pageSize);

    public async ValueTask StoreAsync(BlogPost entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await repository.StoreAsync(entity);

        if (!string.IsNullOrEmpty(entity.Id))
        {
            await fusionCache.RemoveAsync(entity.Id);
        }
    }

    public ValueTask StoreBulkAsync(IReadOnlyCollection<BlogPost> blogPosts) => repository.StoreBulkAsync(blogPosts);

    public async ValueTask DeleteAsync(string id)
    {
        await repository.DeleteAsync(id);
        await fusionCache.RemoveAsync(id);
    }
}
