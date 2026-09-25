using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCronJob;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class ShowBlogPostPageTests : BunitContext
{
    public ShowBlogPostPageTests()
    {
        ComponentFactories.Add<SimilarBlogPostSection, SimilarBlogPostSectionStub>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped(_ => Substitute.For<IBlogPostRepository>());
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        Services.AddScoped(_ => Substitute.For<ITagQueryService>());
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        AddAuthorization();
        ComponentFactories.Add<PageTitle, PageTitleStub>();
        ComponentFactories.Add<Like, LikeStub>();
        ComponentFactories.AddStub<CommentSection>();
    }
    
    [Fact]
    public void ShouldShowLoadingAnimation()
    {
        const string blogPostId = "2";
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        Services.AddScoped(_ => pageQueryMock);
        pageQueryMock.GetAsync(blogPostId)
            .Returns(new ValueTask<BlogPostPage?>(Task.Run(async () =>
            {
                await Task.Delay(250, cancellationToken: TestContext.Current.CancellationToken);
                return (BlogPostPage?)PageOf(new BlogPostBuilder().Build());
            })));


        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPostId));

        cut.FindComponents<Loading>().Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldTrackVisitOnlyOncePerPageView()
    {
        var userRecordService = Substitute.For<IUserRecordService>();
        Services.AddScoped(_ => userRecordService);
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        pageQueryMock.GetAsync("1").Returns(PageOf(new BlogPostBuilder().Build()));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.WaitForAssertion(() => userRecordService.Received(1).StoreUserRecordAsync());
    }

    [Fact]
    public void ShouldSetTitleToTag()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").Build();
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var pageTitleStub = cut.FindComponent<PageTitleStub>();
        var pageTitle = Render(pageTitleStub.Instance.ChildContent!);
        pageTitle.Markup.ShouldBe("Title");
    }

    [Theory]
    [InlineData("url1", null, "url1")]
    [InlineData("url1", "url2", "url2")]
    public void ShouldUseFallbackAsOgDataIfAvailable(string preview, string? fallback, string expected)
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithPreviewImageUrl(preview)
            .WithPreviewImageUrlFallback(fallback)
            .Build();
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindComponent<OgData>().Instance.AbsolutePreviewImageUrl.ShouldBe(expected);
    }

    [Fact]
    public void ShowTagWithLinksWhenAvailable()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithTags("tag1")
            .Build();
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var aElement = cut.Find(".goto-tag") as IHtmlAnchorElement;
        aElement.ShouldNotBeNull();
        aElement.Href.ShouldContain("/searchByTag/tag1");
    }

    [Fact]
    public void ShowNotShowTagsWhenNotSet()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .Build();
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll(".goto-tag").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowReadingIndicatorWhenEnabled(bool isEnabled)
    {
        var appConfiguration = new ApplicationConfigurationBuilder()
            .WithShowReadingIndicator(isEnabled)
            .Build();
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .Build();
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);
        Services.AddScoped(_ => Options.Create(appConfiguration));

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.HasComponent<ReadingIndicator>().ShouldBe(isEnabled);
    }

    [Fact]
    public void ShouldSetCanoncialUrlOfOgDataWithoutSlug()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithTitle("sample")
            .Build();
        blogPost.Id = "1";
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);
        
        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));
        
        cut.FindComponent<OgData>().Instance.CanonicalRelativeUrl.ShouldBe("blogPost/1/sample");
    }

    [Fact]
    public void ShouldRenderStructuredDataIntoPageBody()
    {
        // Regression: the JSON-LD block used to be nested inside OgData's HeadContent.
        // HeadOutlet does not emit script tags, so it never reached the rendered page.
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithTitle("sample")
            .Build();
        blogPost.Id = "1";
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var script = cut.Find("script[type='application/ld+json']");
        script.TextContent.ShouldContain("sample");
    }

    [Fact]
    public void ShouldShowAuthorNameWhenUseMultiAuthorModeIsTrue()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithAuthorName("Test Author")
            .Build();
        blogPost.Id = "1";
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithUseMultiAuthorMode(true).Build()));

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll("span:contains('Test Author')").ShouldHaveSingleItem();
        cut.FindAll("i.user-tie").ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldNotShowAuthorNameWhenUseMultiAuthorModeIsFalse()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder()
            .WithAuthorName("Test Author")
            .Build();
        blogPost.Id = "1";
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithUseMultiAuthorMode(false).Build()));

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll("span:contains('Test Author')").ShouldBeEmpty();
        cut.FindAll("i.user-tie").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotShowAuthorNameWhenAuthorNameIsNull()
    {
        var pageQueryMock = Substitute.For<IBlogPostPageQuery>();
        var blogPost = new BlogPostBuilder().Build(); // Author name is null here.
        blogPost.Id = "1";
        pageQueryMock.GetAsync("1").Returns(PageOf(blogPost));
        Services.AddScoped(_ => pageQueryMock);
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithUseMultiAuthorMode(true).Build()));

        var cut = Render<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll("span:contains('Test Author')").ShouldBeEmpty();
        cut.FindAll("i.user-tie").ShouldBeEmpty();
    }

    private static BlogPostPage PageOf(BlogPost blogPost) => new(blogPost, [], []);

    private class PageTitleStub : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; } = default!;
    }
    
    private class LikeStub : ComponentBase
    {
        [Parameter]
        public BlogPost BlogPost { get; set; } = default!;

        [Parameter]
        public EventCallback<bool> OnBlogPostLiked { get; set; } = default!;
    }
    
    private class SimilarBlogPostSectionStub : ComponentBase
    {
        [Parameter]
        public IReadOnlyList<BlogPostSummary> SimilarBlogPosts { get; set; } = default!;
    }
}
