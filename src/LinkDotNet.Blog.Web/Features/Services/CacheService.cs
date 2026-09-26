using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class CacheService : ICacheInvalidator
{
    private readonly IFusionCache fusionCache;

    public CacheService(IFusionCache fusionCache)
    {
        this.fusionCache = fusionCache;
    }

    public Task ClearCacheAsync() => fusionCache.ClearAsync().AsTask();

    public Task ClearBlogPostPagesAsync() => fusionCache.RemoveByTagAsync(BlogPostCacheKeys.PagesTag).AsTask();
}
