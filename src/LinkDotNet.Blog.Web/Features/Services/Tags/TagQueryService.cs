using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Raven.Client.Documents.Operations.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Features.Services.Tags;

public sealed class TagQueryService(
    IRepository<BlogPost> blogPostRepository,
    IFusionCache fusionCache,
    IOptions<ApplicationConfiguration> appConfiguration) : ITagQueryService
{
    private const string TagCacheKey = "TagUsageList";

    public async Task<IReadOnlyList<TagCount>> GetAllOrderedByUsageAsync()
    {
        return await fusionCache.GetOrSetAsync(
        TagCacheKey,
        async _ => await LoadTagsAsync(),
        options =>
        {
            options.SetDuration(TimeSpan.FromMinutes(
                appConfiguration.Value.FirstPageCacheDurationInMinutes));
        });
    }

    private async Task<IReadOnlyList<TagCount>> LoadTagsAsync()
    {
        var posts = await blogPostRepository.GetAllAsync();

        var tagCounts = posts
            .SelectMany(p => p.Tags ?? Enumerable.Empty<string>())
            .Where(tag => !string.IsNullOrEmpty(tag))
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
