using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using NCronJob;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features;

public class PopularTagsJob : IJob
{
    private const string CacheKey = "PopularTags";
    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IFusionCache fusionCache;

    public PopularTagsJob(
        IRepository<BlogPost> blogPostRepository,
        IFusionCache fusionCache)
    {
        this.blogPostRepository = blogPostRepository;
        this.fusionCache = fusionCache;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        // Use projection to only fetch tags, not full blog posts
        var blogPostTags = await blogPostRepository.GetAllByProjectionAsync(
            bp => bp.Tags,
            filter: bp => bp.IsPublished,
            pageSize: 100  // Limit for performance
        );

        // Count tag frequency
        var tagFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in blogPostTags)
        {
            foreach (var tag in tags)
            {
                var normalizedTag = tag.Trim();
                if (!string.IsNullOrWhiteSpace(normalizedTag))
                {
                    if (tagFrequency.TryGetValue(normalizedTag, out var count))
                    {
                        tagFrequency[normalizedTag] = count + 1;
                    }
                    else
                    {
                        tagFrequency[normalizedTag] = 1;
                    }
                }
            }
        }

        // Get top 12 tags sorted by frequency
        var popularTags = tagFrequency
            .OrderByDescending(kvp => kvp.Value)
            .Take(12)
            .Select(kvp => kvp.Key)
            .ToList();

        // Cache the popular tags for 1 hour
        await fusionCache.SetAsync(
            CacheKey,
            popularTags,
            options => options.SetDuration(TimeSpan.FromHours(1)),
            token: token);
    }

    public static async Task<List<string>> GetPopularTagsAsync(IFusionCache fusionCache, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(fusionCache);
        return await fusionCache.GetOrDefaultAsync<List<string>>(CacheKey, token: token) ?? new List<string>();
    }
}
