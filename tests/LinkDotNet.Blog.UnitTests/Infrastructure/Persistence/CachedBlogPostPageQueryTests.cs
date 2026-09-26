using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Services;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence;

public sealed class CachedBlogPostPageQueryTests : IDisposable
{
    private readonly IBlogPostPageQuery pageQueryMock = Substitute.For<IBlogPostPageQuery>();
    private readonly FusionCache fusionCache = new(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker());
    private readonly CachedBlogPostPageQuery sut;

    public CachedBlogPostPageQueryTests()
    {
        sut = new CachedBlogPostPageQuery(pageQueryMock, fusionCache);
    }

    [Fact]
    public async Task ShouldServeSecondRequestFromCache()
    {
        var page = new BlogPostPage(new BlogPostBuilder().Build(), [], []);
        pageQueryMock.GetAsync("id").Returns(page);

        var first = await sut.GetAsync("id");
        var second = await sut.GetAsync("id");

        first.ShouldBe(page);
        second.ShouldBe(page);
        await pageQueryMock.Received(1).GetAsync("id");
    }

    [Fact]
    public async Task ShouldReloadAfterBlogPostPagesWereCleared()
    {
        pageQueryMock.GetAsync("id").Returns(new BlogPostPage(new BlogPostBuilder().Build(), [], []));
        await sut.GetAsync("id");

        await new CacheService(fusionCache).ClearBlogPostPagesAsync();
        await sut.GetAsync("id");

        await pageQueryMock.Received(2).GetAsync("id");
    }

    public void Dispose() => fusionCache.Dispose();
}
