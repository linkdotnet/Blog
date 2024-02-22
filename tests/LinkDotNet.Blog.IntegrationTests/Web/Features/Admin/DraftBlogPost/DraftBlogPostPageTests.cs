using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.DraftBlogPost;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.DraftBlogPost;

public class DraftBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldOnlyShowPublishedPosts()
    {
        var publishedPost = new BlogPostBuilder().WithTitle("Published").IsPublished().Build();
        var unpublishedPost = new BlogPostBuilder().WithTitle("Not published").IsPublished(false).Build();
        await Repository.StoreAsync(publishedPost);
        await Repository.StoreAsync(unpublishedPost);
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddScoped(_ => Repository);
        var cut = ctx.RenderComponent<DraftBlogPostPage>();
        cut.WaitForElement(".blog-card");

        var blogPosts = cut.FindComponents<ShortBlogPost>();

        blogPosts.Should().HaveCount(1);
        blogPosts[0].Find(".description h1").InnerHtml.Should().Be("Not published");
    }
}
