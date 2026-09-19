using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class BlogPostRepository(IRepository<BlogPost> repository) : IBlogPostRepository
{
    public ValueTask<BlogPost?> GetByIdAsync(string id) => repository.GetByIdAsync(id);

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

    public ValueTask StoreAsync(BlogPost entity) => repository.StoreAsync(entity);

    public ValueTask StoreBulkAsync(IReadOnlyCollection<BlogPost> blogPosts) => repository.StoreBulkAsync(blogPosts);

    public ValueTask DeleteAsync(string id) => repository.DeleteAsync(id);
}
