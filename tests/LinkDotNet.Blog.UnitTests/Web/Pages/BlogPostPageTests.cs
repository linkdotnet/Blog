using System.Threading.Tasks;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Pages;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Services;
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
}