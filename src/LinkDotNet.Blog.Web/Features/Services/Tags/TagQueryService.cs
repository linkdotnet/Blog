using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features.Services.Tags;

public sealed class TagQueryService(
    IRepository<BlogPost> blogPostRepository,
    IFusionCache fusionCache,
    IOptions<ApplicationConfiguration> appConfiguration) : ITagQueryService
{
    private const string TagCacheKey = "TagUsageList";
    private const string PublishedTagCacheKey = "PublishedTagUsageList";

    public async Task<IReadOnlyList<TagCount>> GetAllOrderedByUsageAsync() =>
        await GetCachedAsync(TagCacheKey, onlyPublished: false);

    public async Task<IReadOnlyList<TagCount>> GetPublishedOrderedByUsageAsync() =>
        await GetCachedAsync(PublishedTagCacheKey, onlyPublished: true);

    public async Task ClearTagCacheAsync()
    {
        await fusionCache.RemoveAsync(TagCacheKey);
        await fusionCache.RemoveAsync(PublishedTagCacheKey);
    }

    private async Task<IReadOnlyList<TagCount>> GetCachedAsync(string cacheKey, bool onlyPublished)
    {
        return await fusionCache.GetOrSetAsync(
        cacheKey,
        async _ => await LoadTagsAsync(onlyPublished),
        options =>
        {
            options.SetDuration(TimeSpan.FromMinutes(
                appConfiguration.Value.FirstPageCacheDurationInMinutes));
        });
    }

    private async Task<IReadOnlyList<TagCount>> LoadTagsAsync(bool onlyPublished)
    {
        Expression<Func<BlogPost, bool>>? filter = onlyPublished ? p => p.IsPublished : null;
        var tagLists = await blogPostRepository.GetAllByProjectionAsync(p => p.Tags, filter);

        var tagCounts = tagLists
            .SelectMany(tags => tags ?? Enumerable.Empty<string>())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .GroupBy(tag => tag.Trim())
            .Select(group => new TagCount(
                group.Key,
                group.Count()))
            .OrderByDescending(tc => tc.Count)
            .ThenBy(tc => tc.Name)
            .ToList();

        return tagCounts;
    }
}
