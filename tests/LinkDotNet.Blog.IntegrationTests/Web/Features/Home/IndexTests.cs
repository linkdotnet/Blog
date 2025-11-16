using System.Linq;
using System.Threading.Tasks;
using Bunit.Extensions.WaitForHelpers;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Home;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Home;

public class IndexTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldShowAllBlogPostsWithLatestOneFirst()
    {
        var oldestBlogPost = new BlogPostBuilder().WithTitle("Old").Build();
        var newestBlogPost = new BlogPostBuilder().WithTitle("New").Build();
        await Repository.StoreAsync(oldestBlogPost);
        await Repository.StoreAsync(newestBlogPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
        var cut = ctx.Render<Index>();
        cut.WaitForElement(".blog-card");

        var blogPosts = cut.FindComponents<ShortBlogPost>();

        blogPosts.Count.ShouldBe(2);
        blogPosts[0].Find(".description h4").InnerHtml.ShouldBe("New");
        blogPosts[1].Find(".description h4").InnerHtml.ShouldBe("Old");
    }

    [Fact]
    public async Task ShouldOnlyShowPublishedPosts()
    {
        var publishedPost = new BlogPostBuilder().WithTitle("Published").IsPublished().Build();
        var unpublishedPost = new BlogPostBuilder().WithTitle("Not published").IsPublished(false).Build();
        await Repository.StoreAsync(publishedPost);
        await Repository.StoreAsync(unpublishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
        var cut = ctx.Render<Index>();
        cut.WaitForElement(".blog-card");

        var blogPosts = cut.FindComponents<ShortBlogPost>();

        blogPosts.ShouldHaveSingleItem();
        blogPosts[0].Find(".description h4").InnerHtml.ShouldBe("Published");
    }

    [Fact]
    public async Task ShouldOnlyLoadTenEntities()
    {
        await CreatePublishedBlogPosts(11);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
        var cut = ctx.Render<Index>();
        cut.WaitForElement(".blog-card");

        var blogPosts = cut.FindComponents<ShortBlogPost>();

        blogPosts.Count.ShouldBe(10);
    }

    [Fact]
    public async Task ShouldLoadOnlyItemsInPage()
    {
        await CreatePublishedBlogPosts(11);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);

        var cut = ctx.Render<Index>(
            p => p.Add(s => s.Page, 2));

        cut.WaitForElement(".blog-card");
        var blogPosts = cut.FindComponents<ShortBlogPost>();
        blogPosts.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task ShouldLoadTags()
    {
        var publishedPost = new BlogPostBuilder()
            .WithTitle("Published")
            .IsPublished()
            .WithTags("C Sharp", "Tag2")
            .Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
        var cut = ctx.Render<Index>();
        cut.WaitForElement(".blog-card");

        var tags = cut.FindComponent<ShortBlogPost>().FindAll(".goto-tag");

        tags.Count.ShouldBe(2);
        tags.Select(t => t.TextContent).ShouldContain("C Sharp");
        tags.Select(t => t.TextContent).ShouldContain("Tag2");
        tags.Select(t => t.Attributes.Single(a => a.Name == "href").Value).ShouldContain("/searchByTag/C%20Sharp");
        tags.Select(t => t.Attributes.Single(a => a.Name == "href").Value).ShouldContain("/searchByTag/Tag2");
    }

    [Theory]
    [InlineData("relative/url", "http://localhost/relative/url")]
    [InlineData("http://localhost/relative/url", "http://localhost/relative/url")]
    public void ShouldSetAbsoluteUriForOgData(string givenUri, string expectedUri)
    {
        using var ctx = new BunitContext();
        RegisterComponents(ctx, givenUri);

        var cut = ctx.Render<Index>();

        cut.WaitForComponent<OgData>().Instance.AbsolutePreviewImageUrl.ShouldBe(expectedUri);
    }

    [Theory]
    [InlineData(null!)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldSetPageToFirstIfOutOfRange(int? page)
    {
        await CreatePublishedBlogPosts(11);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);

        var cut = ctx.Render<Index>(p => p.Add(
            i => i.Page, page));

        cut.WaitForElement(".blog-card");
        cut.FindAll(".blog-card").Count.ShouldBe(10);
    }

    [Fact]
    public async Task ShouldShowAuthorNameWhenUseMultiAuthorModeIsTrue()
    {
        var publishedPost = new BlogPostBuilder()
            .WithAuthorName("Test Author")
            .Build();

        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx, useMultiAuthorMode: true);
        var cut = ctx.Render<Index>();

        cut.WaitForElement("li:contains('Test Author')");
        cut.FindAll("li:contains('Test Author')").ShouldHaveSingleItem();
        cut.FindAll("i.user-tie").ShouldHaveSingleItem();
    }

    [Fact]
    public async Task ShouldNotShowAuthorNameWhenUseMultiAuthorModeIsFalse()
    {
        var publishedPost = new BlogPostBuilder()
            .WithAuthorName("Test Author")
            .Build();

        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx, useMultiAuthorMode: false);
        var cut = ctx.Render<Index>();

        var func = () => cut.WaitForElement("li:contains('Test Author')");
        func.ShouldThrow<WaitForFailedException>();
        cut.FindAll("li:contains('Test Author')").ShouldBeEmpty();
        cut.FindAll("i.user-tie").ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldNotShowAuthorNameWhenAuthorNameIsNull()
    {
        var publishedPost = new BlogPostBuilder().Build(); // Author name is null here.
        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx, useMultiAuthorMode: true);
        var cut = ctx.Render<Index>();

        var func = () => cut.WaitForElement("li:contains('Test Author')");
        func.ShouldThrow<WaitForFailedException>();
        cut.FindAll("li:contains('Test Author')").ShouldBeEmpty();
        cut.FindAll("i.user-tie").ShouldBeEmpty();
    }

    private static (ApplicationConfiguration ApplicationConfiguration, Introduction Introduction)
        CreateSampleAppConfiguration(string? profilePictureUri = null, bool useMultiAuthorMode = false)
    {
        return (new ApplicationConfigurationBuilder()
                .WithBlogName(string.Empty)
                .WithBlogPostsPerPage(10)
                .WithUseMultiAuthorMode(useMultiAuthorMode)
                .Build(),
            new Introduction
            {
                Description = string.Empty,
                BackgroundUrl = string.Empty,
                ProfilePictureUrl = profilePictureUri ?? string.Empty,
            });
    }

    private async Task CreatePublishedBlogPosts(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var blogPost = new BlogPostBuilder().IsPublished().Build();
            await Repository.StoreAsync(blogPost);
        }
    }

    private void RegisterComponents(BunitContext ctx, string? profilePictureUri = null, bool useMultiAuthorMode = false)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Options.Create(CreateSampleAppConfiguration(profilePictureUri, useMultiAuthorMode).ApplicationConfiguration));
        ctx.Services.AddScoped(_ => Options.Create(CreateSampleAppConfiguration(profilePictureUri, useMultiAuthorMode).Introduction));
        ctx.Services.AddScoped(_ => Substitute.For<ICacheTokenProvider>());
        ctx.Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        ctx.Services.AddScoped<IFusionCache>(_ => new FusionCache(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker()));
    }
}
