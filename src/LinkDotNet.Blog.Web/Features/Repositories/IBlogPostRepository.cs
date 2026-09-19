using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IBlogPostRepository
{
    ValueTask<BlogPost?> GetByIdAsync(string id);

    ValueTask<IPagedList<BlogPost>> GetAllAsync(
        Expression<Func<BlogPost, bool>>? filter = null,
        Expression<Func<BlogPost, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
        Expression<Func<BlogPost, TProjection>> selector,
        Expression<Func<BlogPost, bool>>? filter = null,
        Expression<Func<BlogPost, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = int.MaxValue);

    ValueTask StoreAsync(BlogPost entity);

    ValueTask StoreBulkAsync(IReadOnlyCollection<BlogPost> blogPosts);

    ValueTask DeleteAsync(string id);
}
