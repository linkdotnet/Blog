using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence;

public class CachedRepositoryTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldNotCacheWhenDifferentQueries()
    {
        var bp1 = new BlogPostBuilder().WithTags("tag 1").Build();
        var bp2 = new BlogPostBuilder().WithTags("tag 2").Build();
        var bp3 = new BlogPostBuilder().WithTags("tag 1").Build();
        await Repository.StoreAsync(bp1);
        await Repository.StoreAsync(bp2);
        await Repository.StoreAsync(bp3);
        var searchTerm = "tag 1";
        var sut = new CachedRepository<BlogPost>(Repository, new MemoryCache(new MemoryCacheOptions()));
        await sut.GetAllAsync(f => f.Tags.Any(t => t.Content == searchTerm));
        searchTerm = "tag 2";

        var allWithTag2 = await sut.GetAllAsync(f => f.Tags.Any(t => t.Content == searchTerm));

        allWithTag2.Count.Should().Be(1);
        allWithTag2.Single().Tags.Single().Content.Should().Be("tag 2");
    }

    [Fact]
    public async Task ShouldResetOnDelete()
    {
        var bp1 = new BlogPostBuilder().WithTitle("1").Build();
        var bp2 = new BlogPostBuilder().WithTitle("2").Build();
        var sut = new CachedRepository<BlogPost>(Repository, new MemoryCache(new MemoryCacheOptions()));
        await sut.StoreAsync(bp1);
        await sut.StoreAsync(bp2);
        await sut.GetAllAsync();
        await sut.DeleteAsync(bp1.Id);

        var db = await sut.GetAllAsync();

        db.Single().Title.Should().Be("2");
    }

    [Fact]
    public async Task ShouldResetOnSave()
    {
        var bp1 = new BlogPostBuilder().WithTitle("1").Build();
        var bp2 = new BlogPostBuilder().WithTitle("2").Build();
        var sut = new CachedRepository<BlogPost>(Repository, new MemoryCache(new MemoryCacheOptions()));
        await sut.StoreAsync(bp1);
        await sut.GetAllAsync();
        bp1.Update(bp2);
        await sut.StoreAsync(bp1);

        var db = await sut.GetAllAsync();

        db.Single().Title.Should().Be("2");
    }
}
