using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services.Tags;
public sealed class TagQueryServiceTests
{

    private readonly IRepository<BlogPost> repository;
    private readonly TagQueryService tagQueryService;

    public TagQueryServiceTests()
    {
        repository = Substitute.For<IRepository<BlogPost>>();
        tagQueryService = new TagQueryService(repository);
    }

    [Fact]
    public async Task ShouldReturnEmptyWhenNoPosts()
    {
        // Arrange
        repository.GetAllAsync()
            .Returns(PagedList<BlogPost>.Empty);

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task AggregatesAndSortsTagsByUsage()
    {
        // Arrange
        var posts = new List<BlogPost>
        {
            new BlogPostBuilder().WithTags("CSharp", "Blazor", "DotNet").Build(),
            new BlogPostBuilder().WithTags("CSharp", "Blazor").Build(),
            new BlogPostBuilder().WithTags("CSharp").Build(),
        };

        repository.GetAllAsync()
            .Returns(CreatePagedList(posts));

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
        var posts = new List<BlogPost>
        {
            new BlogPostBuilder().WithTags("CSharp", " ").Build(),
            new BlogPostBuilder().Build(),
        };

        repository.GetAllAsync()
            .Returns(CreatePagedList(posts));

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
        var posts = new List<BlogPost>
        {
            new BlogPostBuilder().WithTags("CSharp").Build(),
            new BlogPostBuilder().WithTags("Blazor").Build(),
        };

        repository.GetAllAsync()
            .Returns(CreatePagedList(posts));

        // Act
        var result = await tagQueryService.GetAllOrderedByUsageAsync();

        // Assert
        result[0].Name.ShouldBe("Blazor");
        result[1].Name.ShouldBe("CSharp");
    }

    private static PagedList<BlogPost> CreatePagedList(List<BlogPost> posts)
    {
        return new PagedList<BlogPost>(
                posts,
                posts.Count,
                1,
                posts.Count == 0 ? 1 : posts.Count);
    }
}
