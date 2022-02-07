using System.Threading.Tasks;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Pages;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class BlogPostPageTests : TestContext
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

        var cut = RenderComponent<BlogPostPage>(
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
        Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        Services.AddScoped(_ => Mock.Of<IToastService>());
        Services.AddScoped(_ => Mock.Of<AppConfiguration>());
        this.AddTestAuthorization();
        ComponentFactories.AddStub<PageTitle>();
        ComponentFactories.AddStub<Like>();
        ComponentFactories.AddStub<CommentSection>();

        var cut = RenderComponent<BlogPostPage>(
            p => p.Add(s => s.BlogPostId, "1"));

        var pageTitleStub = cut.FindComponent<Stub<PageTitle>>();
        var pageTitle = Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent));
        pageTitle.Markup.Should().Be("Title");
    }
}