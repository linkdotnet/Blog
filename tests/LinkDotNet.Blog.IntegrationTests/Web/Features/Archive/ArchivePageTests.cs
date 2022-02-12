using System;
using System.Threading.Tasks;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Archive;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Archive;

public class ArchivePageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldDisplayAllBlogPosts()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2022, 1, 1), "Blog Post 3"));

        var cut = ctx.RenderComponent<ArchivePage>();

        cut.WaitForState(() => cut.FindAll("h2").Count == 2);
        var yearHeader = cut.FindAll("h2");
        yearHeader.Should().HaveCount(2);
        yearHeader[0].TextContent.Should().Be("2022");
        yearHeader[1].TextContent.Should().Be("2021");
        var entries = cut.FindAll("li");
        entries.Should().HaveCount(3);
        entries[0].TextContent.Should().Be("Blog Post 3");
        entries[1].TextContent.Should().Be("Blog Post 2");
        entries[2].TextContent.Should().Be("Blog Post 1");
    }

    [Fact]
    public async Task ShouldOnlyShowPublishedBlogPosts()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        var publishedBlogPost = new BlogPostBuilder().WithUpdatedDate(new DateTime(2022, 1, 1)).IsPublished().Build();
        var unPublishedBlogPost = new BlogPostBuilder().WithUpdatedDate(new DateTime(2022, 1, 1)).IsPublished(false).Build();
        await Repository.StoreAsync(publishedBlogPost);
        await Repository.StoreAsync(unPublishedBlogPost);

        var cut = ctx.RenderComponent<ArchivePage>();

        cut.WaitForElements("h2");
        cut.FindAll("h2").Should().HaveCount(1);
    }

    private static BlogPost CreateBlogPost(DateTime date, string title)
    {
        return new BlogPostBuilder()
            .WithTitle(title)
            .WithUpdatedDate(date)
            .Build();
    }
}