using System;
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
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2022, 1, 1), "Blog Post 3"));

        var cut = ctx.RenderComponent<ArchivePage>();

        cut.WaitForElements("h2");
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

    [Fact]
    public async Task ShouldShowTotalAmountOfBlogPosts()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"));
        await Repository.StoreAsync(CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"));

        var cut = ctx.RenderComponent<ArchivePage>();

        cut.WaitForElements("h2");
        cut.Find("h3").TextContent.Should().Be("Archive (2 posts)");
    }

    [Fact]
    public void ShouldShowLoading()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => new SlowRepository());

        var cut = ctx.RenderComponent<ArchivePage>();

        cut.FindComponents<Loading>().Count.Should().Be(1);
    }

    [Fact]
    public void ShouldSetOgData()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);

        var cut = ctx.RenderComponent<ArchivePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.Title.Should().Contain("Archive");
    }

    private static BlogPost CreateBlogPost(DateTime date, string title)
    {
        return new BlogPostBuilder()
            .WithTitle(title)
            .WithUpdatedDate(date)
            .Build();
    }

    private sealed class SlowRepository : IRepository<BlogPost>
    {
        public ValueTask<HealthCheckResult> PerformHealthCheckAsync() => throw new NotImplementedException();

        public ValueTask<BlogPost> GetByIdAsync(string id) => throw new NotImplementedException();

        public ValueTask<IPagedList<BlogPost>> GetAllAsync(
            Expression<Func<BlogPost, bool>> filter = null,
            Expression<Func<BlogPost, object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = int.MaxValue) => throw new NotImplementedException();

        public async ValueTask<IPagedList<TProjection>> GetAllByProjectionAsync<TProjection>(
            Expression<Func<BlogPost, TProjection>> selector,
            Expression<Func<BlogPost, bool>> filter = null,
            Expression<Func<BlogPost, object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = int.MaxValue)
        {
            await Task.Delay(250);
            return PagedList<TProjection>.Empty;
        }

        public ValueTask StoreAsync(BlogPost entity) => throw new NotImplementedException();

        public ValueTask DeleteAsync(string id) => throw new NotImplementedException();
    }
}
