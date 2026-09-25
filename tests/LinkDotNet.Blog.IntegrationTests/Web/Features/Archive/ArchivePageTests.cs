using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Archive;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Archive;

public class ArchivePageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldDisplayAllBlogPosts()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped<IBlogPostListQuery>(_ => new BlogPostListQuery(DbContextFactory));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2022, 1, 1), "Blog Post 3"));

        var cut = ctx.Render<ArchivePage>();

        cut.WaitForElements("h2");
        var yearHeader = cut.FindAll("h2");
        yearHeader.Count.ShouldBe(2);
        yearHeader[0].TextContent.ShouldContain("2022");
        yearHeader[1].TextContent.ShouldContain("2021");
        var entries = cut.FindAll("li");
        entries.Count.ShouldBe(3);
        entries[0].TextContent.ShouldContain("Blog Post 3");
        entries[1].TextContent.ShouldContain("Blog Post 2");
        entries[2].TextContent.ShouldContain("Blog Post 1");
    }

    [Fact]
    public async Task ShouldOnlyShowPublishedBlogPosts()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped<IBlogPostListQuery>(_ => new BlogPostListQuery(DbContextFactory));
        var publishedBlogPost = new BlogPostBuilder().WithUpdatedDate(new DateTime(2022, 1, 1)).IsPublished().Build();
        var unPublishedBlogPost = new BlogPostBuilder().WithUpdatedDate(new DateTime(2022, 1, 1)).IsPublished(false).Build();
        await Repository.StoreAsync(publishedBlogPost);
        await Repository.StoreAsync(unPublishedBlogPost);

        var cut = ctx.Render<ArchivePage>();

        cut.WaitForElements("h2");
        cut.FindAll("h2").ShouldHaveSingleItem();
    }

    [Fact]
    public async Task ShouldShowTotalAmountOfBlogPosts()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped<IBlogPostListQuery>(_ => new BlogPostListQuery(DbContextFactory));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"));

        var cut = ctx.Render<ArchivePage>();

        cut.WaitForElements("h2");
        cut.Find("h3").TextContent.ShouldBe("Archive (2 posts)");
    }

    [Fact]
    public void ShouldShowLoading()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped<IBlogPostListQuery>(_ => new SlowListQuery());

        var cut = ctx.Render<ArchivePage>();

        cut.FindComponents<Loading>().Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldSetOgData()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped<IBlogPostListQuery>(_ => new BlogPostListQuery(DbContextFactory));

        var cut = ctx.Render<ArchivePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.Title.ShouldContain("Archive");
    }

    private static BlogPost CreateBlogPost(DateTime date, string title)
    {
        return new BlogPostBuilder()
            .WithTitle(title)
            .WithUpdatedDate(date)
            .Build();
    }

    private sealed class SlowListQuery : IBlogPostListQuery
    {
        public async ValueTask<IPagedList<BlogPostSummary>> GetPublishedAsync(int page = 1, int pageSize = int.MaxValue)
        {
            await Task.Delay(250);
            return PagedList<BlogPostSummary>.Empty;
        }

        public ValueTask<IReadOnlyList<BlogPostSummary>> GetPublishedByTagAsync(string tag) => throw new NotImplementedException();

        public ValueTask<IReadOnlyList<BlogPostSummary>> SearchPublishedAsync(string term) => throw new NotImplementedException();

        public ValueTask<IReadOnlyList<BlogPostSummary>> GetDraftsAsync() => throw new NotImplementedException();

        public ValueTask<IReadOnlyList<BlogPostSummary>> GetByIdsAsync(IReadOnlyCollection<string> ids) => throw new NotImplementedException();
    }
}
