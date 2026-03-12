using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;


namespace LinkDotNet.Blog.UnitTests.Web.Features.Services.Tags;
public sealed class TagQueryServiceTests
{

    private readonly IRepository<BlogPost> repository;
    private readonly TagQueryService tagQueryService;
    private readonly IFusionCache fusionCache;

    public TagQueryServiceTests()
    {
        repository = Substitute.For<IRepository<BlogPost>>();

        fusionCache = new FusionCache(
            new FusionCacheOptions(),
            logger: null,
            memoryCache: new MemoryCache(new MemoryCacheOptions())
        );

        var config = Options.Create(new ApplicationConfigurationBuilder().Build());

        tagQueryService = new TagQueryService(repository, fusionCache, config);
    }

    [Fact]
    public async Task ShouldReturnEmptyWhenNoPosts()
    {
        // Arrange
        repository.GetAllByProjectionAsync(p => p.Tags)
            .ReturnsForAnyArgs(new PagedList<List<string>>([], 0, 1, 1));

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task AggregatesAndSortsTagsByUsage()
    {
        // Arrange
        var tagLists = new List<List<string>>
        {
            new() { "CSharp", "Blazor", "DotNet" },
            new() { "CSharp", "Blazor" },
            new() { "CSharp" },
        };

        repository.GetAllByProjectionAsync(p => p.Tags)
            .ReturnsForAnyArgs(new PagedList<List<string>>(
                tagLists, tagLists.Count, 1, tagLists.Count));

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result.Count.ShouldBe(3);

        result[0].Name.ShouldBe("CSharp");
        result[0].Count.ShouldBe(3);

        result[1].Name.ShouldBe("Blazor");
        result[1].Count.ShouldBe(2);

        result[2].Name.ShouldBe("DotNet");
        result[2].Count.ShouldBe(1);
    }

    [Fact]
    public async Task ShouldIgnoreNullOrWhitespaceTags()
    {
        // Arrange
        var tagLists = new List<List<string>>
        {
            new() { "CSharp", " " },
            new() 
        };

        repository.GetAllByProjectionAsync(p => p.Tags)
            .ReturnsForAnyArgs(new PagedList<List<string>>(
                tagLists, tagLists.Count, 1, tagLists.Count ));

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("CSharp");
        result[0].Count.ShouldBe(1);
    }

    [Fact]
    public async Task ShouldSortAlphabeticallyWhenCountsAreEqual()
    {
        // Arrange
        var tagLists = new List<List<string>>
        {
            new() { "CSharp" },
            new() { "Blazor" }
        };

        repository.GetAllByProjectionAsync(p => p.Tags)
            .ReturnsForAnyArgs(new PagedList<List<string>>(
                tagLists, tagLists.Count, 1, tagLists.Count));

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result[0].Name.ShouldBe("Blazor");
        result[1].Name.ShouldBe("CSharp");
    }
}
