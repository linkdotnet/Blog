using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests;

public sealed class SmokeTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable, IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly IServiceScope scope;
    private readonly HttpClient client;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("BlogName", "Tests Title");
            builder.UseSetting("PersistenceProvider", PersistenceProvider.Sqlite.Key);
            builder.UseSetting("ConnectionString", "DataSource=file::memory:?cache=shared");
        });
        
        scope = this.factory.Services.CreateScope();
        client = this.factory.CreateClient();
    }

    [Fact]
    public async Task ShouldBootUpApplication()
    {
        var result = await client.GetAsync("/", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldAllowDotsForTagSearch()
    {
        var result = await client.GetAsync("/searchByTag/.NET5", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldAllowDotsForFreeTextSearch()
    {
        var result = await client.GetAsync("/search/.NET5", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task RssFeedShouldBeRateLimited()
    {
        const int numberOfRequests = 16;
        
        for (var i = 0; i < numberOfRequests - 1; i++)
        {
            var result = await client.GetAsync("/feed.rss", cancellationToken: TestContext.Current.CancellationToken);
            result.IsSuccessStatusCode.ShouldBeTrue();
        }
        
        var lastResult = await client.GetAsync("/feed.rss", cancellationToken: TestContext.Current.CancellationToken);
        lastResult.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task ShowingBlogPost_ShouldOnlyHaveOneTitle()
    {
        var blogPost = new BlogPostBuilder().WithTitle("My Blog Post").Build();
        var contextProvider = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BlogDbContext>>();
        await using var context = await contextProvider.CreateDbContextAsync(TestContext.Current.CancellationToken);
        await context.BlogPosts.AddAsync(blogPost, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await client.GetAsync($"/blogPost/{blogPost.Id}", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
        var content = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var document = GetHtmlDocument(content);
        var titleTags = document.QuerySelectorAll("title");
        titleTags.Length.ShouldBe(1);
        titleTags.Single().TextContent.ShouldBe("My Blog Post");
    }

    [Fact]
    public async Task IndexPage_HasTitleFromConfiguration()
    {
        var result = await client.GetAsync("/", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
        var content = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var document = GetHtmlDocument(content);
        var titleTags = document.QuerySelectorAll("title");
        titleTags.Length.ShouldBe(1);
        titleTags.Single().TextContent.ShouldBe("Tests Title");
    }

    public void Dispose()
    {
        scope.Dispose();
        client.Dispose();
        factory?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        scope.Dispose();
        client.Dispose();
        await factory.DisposeAsync();
    }

    private static IHtmlDocument GetHtmlDocument(string html)
    {
        var parser = new HtmlParser();
        return parser.ParseDocument(html);
    }
}