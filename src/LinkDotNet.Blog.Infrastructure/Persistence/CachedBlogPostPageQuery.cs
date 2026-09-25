using System;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class CachedBlogPostPageQuery : IBlogPostPageQuery
{
    private static readonly TimeSpan PageDuration = TimeSpan.FromDays(7);
    private static readonly TimeSpan MissingPageDuration = TimeSpan.FromMinutes(1);

    private readonly IBlogPostPageQuery blogPostPageQuery;
    private readonly IFusionCache fusionCache;

    public CachedBlogPostPageQuery(IBlogPostPageQuery blogPostPageQuery, IFusionCache fusionCache)
    {
        this.blogPostPageQuery = blogPostPageQuery;
        this.fusionCache = fusionCache;
    }

    public async ValueTask<BlogPostPage?> GetAsync(string blogPostId) =>
        await fusionCache.GetOrSetAsync<BlogPostPage?>(
            BlogPostCacheKeys.Page(blogPostId),
            async (context, _) =>
            {
                var page = await blogPostPageQuery.GetAsync(blogPostId);
                if (page is null)
                {
                    context.Options.Duration = MissingPageDuration;
                }

                return page;
            },
            options: new FusionCacheEntryOptions(PageDuration),
            tags: [BlogPostCacheKeys.PagesTag]);
}
