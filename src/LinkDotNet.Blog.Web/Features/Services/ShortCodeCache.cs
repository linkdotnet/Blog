using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features.Services;

public static class ShortCodeCache
{
    private const string Key = "shortcodes";

    public static async ValueTask<IReadOnlyCollection<ShortCode>> GetAsync(IFusionCache cache, IRepository<ShortCode> repository)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(repository);

        return await cache.GetOrSetAsync<IReadOnlyCollection<ShortCode>>(
            Key,
            async _ => await repository.GetAllAsync(),
            TimeSpan.FromDays(7));
    }

    public static ValueTask RemoveAsync(IFusionCache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);
        return cache.RemoveAsync(Key);
    }
}
