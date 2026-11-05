using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Services.Tags;

public class TagQueryServiceTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldNotExposeTagsOfUnpublishedPostsInPublishedList()
    {
        await Repository.StoreAsync(new BlogPostBuilder().WithTags("Public").IsPublished().Build());
        await Repository.StoreAsync(new BlogPostBuilder().WithTags("Public", "Secret").IsPublished(false).Build());
        var service = CreateService();

        var published = await service.GetPublishedOrderedByUsageAsync();
        var all = await service.GetAllOrderedByUsageAsync();

        published.ShouldHaveSingleItem().ShouldBe(new TagCount("Public", 1));
        all.Count.ShouldBe(2);
    }

    private TagQueryService CreateService()
    {
        var cache = new FusionCache(
            new FusionCacheOptions(),
            logger: null,
            memoryCache: new MemoryCache(new MemoryCacheOptions()));
        return new TagQueryService(Repository, cache, Options.Create(new ApplicationConfigurationBuilder().Build()));
    }
}
