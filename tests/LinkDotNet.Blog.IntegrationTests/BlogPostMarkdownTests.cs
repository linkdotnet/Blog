using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests;

public sealed class BlogPostMarkdownTests : IClassFixture<BlogPostMarkdownTests.MarkdownEnabledFactory>, IDisposable
{
    private const string MarkdownContentType = "text/markdown; charset=utf-8";
    private readonly MarkdownEnabledFactory factory;
    private readonly HttpClient client;

    public BlogPostMarkdownTests(MarkdownEnabledFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/blogPost/{0}")]
    [InlineData("/blogPost/{0}/my-blog-post")]
    [InlineData("/blogPost/{0}/a-wrong-slug")]
    [InlineData("/blogPost/{0}/")]
    [InlineData("/BLOGPOST/{0}")]
    public async Task ShouldServeMarkdownWithAndWithoutSlug(string urlTemplate)
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithTitle("My Blog Post").WithContent("Hello").Build());

        using var response = await GetAsync(client, string.Format(urlTemplate, blogPost.Id), "text/markdown");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().ShouldBe(MarkdownContentType);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldStartWith("---\ntitle: \"My Blog Post\"\n");
        body.ShouldEndWith("---\n\n# My Blog Post\n\nHello");
    }

    [Theory]
    [InlineData("/blogPost/{0}.md")]
    [InlineData("/blogPost/{0}/my-blog-post.md")]
    [InlineData("/blogPost/{0}/a-wrong-slug.MD")]
    [InlineData("/blogPost/{0}/.md")]
    public async Task ShouldServeMarkdownForMdSuffixRegardlessOfAccept(string urlTemplate)
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithTitle("My Blog Post").WithContent("Hello").Build());

        using var response = await GetAsync(client, string.Format(urlTemplate, blogPost.Id), "text/html");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().ShouldBe(MarkdownContentType);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldEndWith("# My Blog Post\n\nHello");
    }

    [Fact]
    public async Task ShouldReturnNotFoundForMdSuffixOfUnpublishedBlogPost()
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().IsPublished(false).Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}.md", null);

        await ShouldBeMarkdownNotFoundAsync(response);
    }

    [Theory]
    [InlineData("/blogPost/{0}", "/blogPost/{0}.md")]
    [InlineData("/blogPost/{0}/some-slug/", "/blogPost/{0}/some-slug.md")]
    public async Task ShouldAdvertiseMarkdownVersionOnHtmlResponse(string urlTemplate, string linkTemplate)
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithTitle("Linked Post").Build());

        using var response = await GetAsync(client, string.Format(urlTemplate, blogPost.Id), null);

        await ShouldBeHtmlBlogPostAsync(response, "Linked Post");
        response.Headers.GetValues("Link").ShouldContain($"<{string.Format(linkTemplate, blogPost.Id)}>; rel=\"alternate\"; type=\"text/markdown\"");
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain($"<link rel=\"alternate\" type=\"text/markdown\" href=\"http://localhost/blogPost/{blogPost.Id}/linked-post.md\" />");
    }

    [Fact]
    public async Task ShouldListPublishedBlogPostsInLlmsTxt()
    {
        var published = await SeedAsync(factory, new BlogPostBuilder().WithTitle("Listed Post").WithShortDescription("**Short**\nsummary").Build());
        await SeedAsync(factory, new BlogPostBuilder().WithTitle("Hidden Draft").IsPublished(false).Build());

        using var response = await GetAsync(client, "/llms.txt", null);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().ShouldBe("text/plain; charset=utf-8");
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldStartWith("# ");
        body.ShouldContain($"- [Listed Post](http://localhost/blogPost/{published.Id}/listed-post.md): Short summary\n");
        body.ShouldNotContain("Hidden Draft");
    }

    [Theory]
    [InlineData("text/markdown")]
    [InlineData("TEXT/MARKDOWN")]
    [InlineData("text/markdown; charset=utf-8")]
    [InlineData("text/markdown, text/html;q=0.9")]
    [InlineData("text/html;q=0.5, text/markdown")]
    [InlineData("text/markdown, */*")]
    public async Task ShouldServeMarkdownWhenPreferred(string accept)
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", accept);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("text/markdown");
    }

    [Theory]
    [InlineData("text/markdown;q=0")]
    [InlineData("text/html, text/markdown;q=0.9")]
    [InlineData("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8")]
    [InlineData("*/*")]
    [InlineData("text/*")]
    [InlineData(null)]
    public async Task ShouldServeHtmlWhenMarkdownIsNotPreferred(string? accept)
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithTitle("Html Post").Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", accept);

        await ShouldBeHtmlBlogPostAsync(response, "Html Post");
        response.Headers.Vary.ShouldContain("Accept");
    }

    [Fact]
    public async Task ShouldServeContentVerbatimWithVaryHeader()
    {
        const string content = "## Heading\n\n```csharp\nvar x = \"<script>alert(1)</script>\";\n```\n\nÄÖÜ ß 🚀 & <b>bold</b>";
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithContent(content).Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", "text/markdown");

        response.Headers.Vary.ShouldContain("Accept");
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldEndWith(content);
    }

    [Fact]
    public async Task ShouldRenderFrontMatter()
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder()
            .WithTitle("Title: with \"quotes\"")
            .WithShortDescription("A short description")
            .WithTags("C#", "Tag: \"x\"")
            .WithUpdatedDate(new DateTime(2026, 9, 1))
            .Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", "text/markdown");

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldStartWith($"""
            ---
            title: "Title: with \"quotes\""
            description: "A short description"
            updated: 2026-09-01
            tags: ["C#", "Tag: \"x\""]
            canonical: "http://localhost/blogPost/{blogPost.Id}/{blogPost.Slug}"
            ---

            # Title: with "quotes"
            """.ReplaceLineEndings("\n"));
    }

    [Fact]
    public async Task ShouldFormatDateIndependentOfRequestCulture()
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithUpdatedDate(new DateTime(2026, 9, 1)).Build());
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/blogPost/{blogPost.Id}");
        request.Headers.Add("Accept", "text/markdown");
        request.Headers.Add("Accept-Language", "ar-SA");

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain("updated: 2026-09-01\n");
    }

    [Fact]
    public async Task ShouldExpandShortCodes()
    {
        var shortCodeName = $"Code{Guid.NewGuid():N}";
        var blogPost = await SeedAsync(
            factory,
            new BlogPostBuilder().WithContent($"Before [[{shortCodeName}]] [[Unknown]] after").Build(),
            ShortCode.Create(shortCodeName, "**expanded**"));

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", "text/markdown");

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldEndWith("Before **expanded** [[Unknown]] after");
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnpublishedBlogPost()
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder().WithContent("Secret draft").IsPublished(false).Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", "text/markdown");

        await ShouldBeMarkdownNotFoundAsync(response);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForScheduledBlogPost()
    {
        var blogPost = await SeedAsync(factory, new BlogPostBuilder()
            .WithContent("Secret draft")
            .IsPublished(false)
            .WithScheduledPublishDate(DateTime.UtcNow.AddDays(1))
            .Build());

        using var response = await GetAsync(client, $"/blogPost/{blogPost.Id}", "text/markdown");

        await ShouldBeMarkdownNotFoundAsync(response);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownBlogPost()
    {
        using var response = await GetAsync(client, $"/blogPost/{Guid.NewGuid()}", "text/markdown");

        await ShouldBeMarkdownNotFoundAsync(response);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/searchByTag/dotnet")]
    [InlineData("/feed.rss")]
    [InlineData("/sitemap.xml")]
    public async Task ShouldNotServeMarkdownForOtherPages(string url)
    {
        using var response = await GetAsync(client, url, "text/markdown");

        response.IsSuccessStatusCode.ShouldBeTrue();
        response.Content.Headers.ContentType!.MediaType.ShouldNotBe("text/markdown");
        response.Headers.Vary.ShouldNotContain("Accept");
    }

    [Theory]
    [InlineData("false")]
    [InlineData(null)]
    public async Task ShouldServeHtmlWhenDisabled(string? enableMarkdownForLlms)
    {
        await using var disabledFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => Configure(builder, enableMarkdownForLlms));
        using var disabledClient = disabledFactory.CreateClient();
        var blogPost = await SeedAsync(disabledFactory, new BlogPostBuilder().WithTitle("Disabled Post").Build());

        using var response = await GetAsync(disabledClient, $"/blogPost/{blogPost.Id}", "text/markdown");

        await ShouldBeHtmlBlogPostAsync(response, "Disabled Post");
        response.Headers.Vary.ShouldNotContain("Accept");
        response.Headers.Contains("Link").ShouldBeFalse();
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldNotContain("text/markdown");

        using var mdSuffixResponse = await GetAsync(disabledClient, $"/blogPost/{blogPost.Id}/disabled-post.md", null);
        await ShouldBeHtmlBlogPostAsync(mdSuffixResponse, "Disabled Post");

        using var llmsTxtResponse = await GetAsync(disabledClient, "/llms.txt", null);
        llmsTxtResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    public void Dispose() => client.Dispose();

    private static void Configure(IWebHostBuilder builder, string? enableMarkdownForLlms)
    {
        builder.UseSetting("PersistenceProvider", PersistenceProvider.Sqlite.Key);
        builder.UseSetting("ConnectionString", $"DataSource=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        if (enableMarkdownForLlms is not null)
        {
            builder.UseSetting("EnableMarkdownForLlms", enableMarkdownForLlms);
        }
    }

    private static async Task<BlogPost> SeedAsync(WebApplicationFactory<Program> app, BlogPost blogPost, params ShortCode[] shortCodes)
    {
        using var scope = app.Services.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BlogDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        await context.BlogPosts.AddAsync(blogPost, TestContext.Current.CancellationToken);
        await context.ShortCodes.AddRangeAsync(shortCodes, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        return blogPost;
    }

    private static async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string url, string? accept)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (accept is not null)
        {
            request.Headers.TryAddWithoutValidation("Accept", accept);
        }

        return await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static async Task ShouldBeHtmlBlogPostAsync(HttpResponseMessage response, string title)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("text/html");
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain($"<title>{title}</title>");
    }

    private static async Task ShouldBeMarkdownNotFoundAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.ToString().ShouldBe(MarkdownContentType);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldBe("# Not found\n");
    }

    public sealed class MarkdownEnabledFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder) => Configure(builder, "true");
    }
}
