using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence;

public sealed class CacheInvalidatingBlogPostRepositoryTests : IDisposable
{
    private readonly IBlogPostRepository repositoryMock = Substitute.For<IBlogPostRepository>();
    private readonly FusionCache fusionCache = new(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker());
    private readonly CacheInvalidatingBlogPostRepository sut;

    public CacheInvalidatingBlogPostRepositoryTests()
    {
        sut = new CacheInvalidatingBlogPostRepository(repositoryMock, fusionCache);
    }

    [Fact]
    public async Task ShouldEvictAllPagesAfterRevision()
    {
        await CachePagesAsync("id", "other");

        await sut.ReviseAsync("id", (_, _) => throw new InvalidOperationException());

        (await IsCachedAsync("id")).ShouldBeFalse();
        (await IsCachedAsync("other")).ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldEvictAllPagesEvenWhenRevisionFails()
    {
        await CachePagesAsync("id");
        repositoryMock.ReviseAsync("id", Arg.Any<Func<BlogPost, int, BlogPostVersion>>())
            .Returns(_ => ValueTask.FromException(new BlogPostRevisionConflictException()));

        await Should.ThrowAsync<BlogPostRevisionConflictException>(async () => await sut.ReviseAsync("id", (_, _) => throw new InvalidOperationException()));

        (await IsCachedAsync("id")).ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldEvictOnlyLikedPage()
    {
        await CachePagesAsync("id", "other");

        await sut.LikeAsync("id");

        await repositoryMock.Received(1).LikeAsync("id");
        (await IsCachedAsync("id")).ShouldBeFalse();
        (await IsCachedAsync("other")).ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldEvictAllPagesAfterDelete()
    {
        await CachePagesAsync("id", "other");

        await sut.DeleteAsync("id");

        await repositoryMock.Received(1).DeleteAsync("id");
        (await IsCachedAsync("id")).ShouldBeFalse();
        (await IsCachedAsync("other")).ShouldBeFalse();
    }

    public void Dispose() => fusionCache.Dispose();

    private async Task CachePagesAsync(params string[] blogPostIds)
    {
        foreach (var blogPostId in blogPostIds)
        {
            await fusionCache.SetAsync(BlogPostCacheKeys.Page(blogPostId), "cached", tags: [BlogPostCacheKeys.PagesTag], token: TestContext.Current.CancellationToken);
        }
    }

    private async Task<bool> IsCachedAsync(string blogPostId) =>
        (await fusionCache.TryGetAsync<string>(BlogPostCacheKeys.Page(blogPostId), token: TestContext.Current.CancellationToken)).HasValue;
}
