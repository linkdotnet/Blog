using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CacheInvalidatingBlogPostRepository : IBlogPostRepository
{
    private readonly IBlogPostRepository repository;
    private readonly IFusionCache fusionCache;

    public CacheInvalidatingBlogPostRepository(IBlogPostRepository repository, IFusionCache fusionCache)
    {
        this.repository = repository;
        this.fusionCache = fusionCache;
    }

    public ValueTask<BlogPost?> GetAsync(string blogPostId) => repository.GetAsync(blogPostId);

    public async ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise)
    {
        try
        {
            await repository.ReviseAsync(blogPostId, revise);
        }
        finally
        {
            await EvictAllPagesAsync();
        }
    }

    public ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId) =>
        repository.GetVersionHistoryAsync(blogPostId);

    public async ValueTask LikeAsync(string blogPostId)
    {
        try
        {
            await repository.LikeAsync(blogPostId);
        }
        finally
        {
            await EvictPageAsync(blogPostId);
        }
    }

    public async ValueTask UnlikeAsync(string blogPostId)
    {
        try
        {
            await repository.UnlikeAsync(blogPostId);
        }
        finally
        {
            await EvictPageAsync(blogPostId);
        }
    }

    public async ValueTask DeleteAsync(string blogPostId)
    {
        try
        {
            await repository.DeleteAsync(blogPostId);
        }
        finally
        {
            await EvictAllPagesAsync();
        }
    }

    private ValueTask EvictPageAsync(string blogPostId) => fusionCache.RemoveAsync(BlogPostCacheKeys.Page(blogPostId));

    // Other pages show this post as a similar-post card, so its title or description may appear there as well.
    private ValueTask EvictAllPagesAsync() => fusionCache.RemoveByTagAsync(BlogPostCacheKeys.PagesTag);
}
