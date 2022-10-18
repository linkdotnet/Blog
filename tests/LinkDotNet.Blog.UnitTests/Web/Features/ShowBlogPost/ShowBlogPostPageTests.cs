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

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class ShowBlogPostPageTests : TestContext
{
    [Fact]
    public void ShouldShowLoadingAnimation()
    {
        const string blogPostId = "2";
        var repositoryMock = new Mock<IRepository<BlogPost>>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped(_ => repositoryMock.Object);
        Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        repositoryMock.Setup(r => r.GetByIdAsync(blogPostId))
            .Returns(async () =>
            {
                await Task.Delay(250);
                return new BlogPostBuilder().Build();
            });

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPostId));

        cut.FindComponents<Loading>().Count.Should().Be(1);
    }

    [Fact]
    public void ShouldSetTitleToTag()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var repositoryMock = new Mock<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").Build();
        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(blogPost);
        Services.AddScoped(_ => repositoryMock.Object);
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
        var repositoryMock = new Mock<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .WithPreviewImageUrl(preview)
            .WithPreviewImageUrlFallback(fallback)
            .Build();
        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(blogPost);
        Services.AddScoped(_ => repositoryMock.Object);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindComponent<OgData>().Instance.AbsolutePreviewImageUrl.Should().Be(expected);
    }

    [Fact]
    public void ShowTagWithLinksWhenAvailable()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var repositoryMock = new Mock<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .WithTags("tag1")
            .Build();
        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(blogPost);
        Services.AddScoped(_ => repositoryMock.Object);
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
        JSInterop.Mode = JSRuntimeMode.Loose;
        var repositoryMock = new Mock<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .Build();
        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(blogPost);
        Services.AddScoped(_ => repositoryMock.Object);
        SetupMocks();

        var cut = RenderComponent<ShowBlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        cut.FindAll(".goto-tag").Should().BeEmpty();
    }

    private void SetupMocks()
    {
        Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        Services.AddScoped(_ => Mock.Of<IToastService>());
        Services.AddScoped(_ => Mock.Of<AppConfiguration>());
        this.AddTestAuthorization();
        ComponentFactories.AddStub<PageTitle>();
        ComponentFactories.AddStub<Like>();
        ComponentFactories.AddStub<CommentSection>();
    }
}