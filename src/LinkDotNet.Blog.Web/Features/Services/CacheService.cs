using System.Threading.Tasks;
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
}
