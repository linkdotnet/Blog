using System;
using System.Net.Http;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web;

public sealed class SitemapControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable, IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly IServiceScope scope;
    private readonly HttpClient client;

    public SitemapControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task GetSitemap_ShouldReturnSuccessStatusCode()
    {
        var result = await client.GetAsync("/sitemap.xml", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task GetSitemap_ShouldReturnXmlContentType()
    {
        var result = await client.GetAsync("/sitemap.xml", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
        result.Content.Headers.ContentType?.MediaType.ShouldBe("application/xml");
    }

    [Fact]
    public async Task GetSitemap_ShouldReturnNonEmptyContent()
    {
        var result = await client.GetAsync("/sitemap.xml", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
        var content = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldNotBeNullOrEmpty();
        content.ShouldContain("<?xml");
    }

    [Fact]
    public async Task GetSitemap_ShouldHaveCacheHeaders()
    {
        var result = await client.GetAsync("/sitemap.xml", cancellationToken: TestContext.Current.CancellationToken);

        result.IsSuccessStatusCode.ShouldBeTrue();
        result.Headers.CacheControl?.MaxAge.ShouldNotBeNull();
        result.Headers.CacheControl?.MaxAge?.TotalSeconds.ShouldBe(3600);
    }

    [Fact]
    public async Task GetSitemap_WithBlogPosts_ShouldIncludeBlogPostUrls()
    {
        // Arrange
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("First Blog Post")
            .WithContent("Content for first post")
            .Build();
        
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("Second Blog Post")
            .WithContent("Content for second post")
            .Build();

        var contextProvider = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BlogDbContext>>();
        await using var context = await contextProvider.CreateDbContextAsync(TestContext.Current.CancellationToken);
        await context.BlogPosts.AddRangeAsync(new[] { blogPost1, blogPost2 }, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await client.GetAsync("/sitemap.xml", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccessStatusCode.ShouldBeTrue();
        var content = await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldContain($"/blogPost/{blogPost1.Id}");
        content.ShouldContain($"/blogPost/{blogPost2.Id}");
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
}
