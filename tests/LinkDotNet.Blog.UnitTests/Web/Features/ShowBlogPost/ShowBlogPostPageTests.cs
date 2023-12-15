using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class ShowBlogPostPageTests : TestContext
{
    [Fact]
    public void ShouldShowLoadingAnimation()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        SetupMocks();
        Services.AddScoped(_ => repositoryMock);
        repositoryMock.GetByIdAsync(blogPostId)
            .Returns(new ValueTask<BlogPost>(Task.Run(async () => 
            {
                await Task.Delay(250);
                return new BlogPostBuilder().Build();
            })));


        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPostId));

        cut.FindComponents<Loading>().Count.Should().Be(1);
    }

    [Fact]
    public void ShouldSetTitleToTag()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").Build();
        repositoryMock.GetByIdAsync("1").Returns(blogPost);
        Services.AddScoped(_ => repositoryMock);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var pageTitleStub = cut.FindComponent<Stub<PageTitle>>();
        var pageTitle = Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent));
        pageTitle.Markup.Should().Be("Title");
    }

    [Theory]
    [InlineData("url1", null, "url1")]
    [InlineData("url1", "url2", "url2")]
    public void ShouldUseFallbackAsOgDataIfAvailable(string preview, string fallback, string expected)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .WithPreviewImageUrl(preview)
            .WithPreviewImageUrlFallback(fallback)
            .Build();
        repositoryMock.GetByIdAsync("1").Returns(blogPost);
        Services.AddScoped(_ => repositoryMock);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindComponent<OgData>().Instance.AbsolutePreviewImageUrl.Should().Be(expected);
    }

    [Fact]
    public void ShowTagWithLinksWhenAvailable()
    {
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .WithTags("tag1")
            .Build();
        repositoryMock.GetByIdAsync("1").Returns(blogPost);
        Services.AddScoped(_ => repositoryMock);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var aElement = cut.Find(".goto-tag").Unwrap() as IHtmlAnchorElement;
        aElement.Should().NotBeNull();
        aElement.Href.Should().Contain("/searchByTag/tag1");
    }

    [Fact]
    public void ShowNotShowTagsWhenNotSet()
    {
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .Build();
        repositoryMock.GetByIdAsync("1").Returns(blogPost);
        Services.AddScoped(_ => repositoryMock);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll(".goto-tag").Should().BeEmpty();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowReadingIndicatorWhenEnabled(bool isEnabled)
    {
        var appConfiguration = new ApplicationConfiguration
        {
            ShowReadingIndicator = isEnabled,
        };
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .Build();
        repositoryMock.GetByIdAsync("1").Returns(blogPost);
        Services.AddScoped(_ => repositoryMock);
        SetupMocks();
        Services.AddScoped(_ => Options.Create(appConfiguration));

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.HasComponent<ReadingIndicator>().Should().Be(isEnabled);
    }

    private void SetupMocks()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration()));
        this.AddTestAuthorization();
        ComponentFactories.AddStub<PageTitle>();
        ComponentFactories.AddStub<Like>();
        ComponentFactories.AddStub<CommentSection>();
    }
}