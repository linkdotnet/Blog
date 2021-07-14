using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages.Admin
{
    public class DraftBlogPostPageTests : SqlDatabaseTestBase
    {
        [Fact]
        public async Task ShouldOnlyShowPublishedPosts()
        {
            var publishedPost = new BlogPostBuilder().WithTitle("Published").IsPublished().Build();
            var unpublishedPost = new BlogPostBuilder().WithTitle("Not published").IsPublished(false).Build();
            await BlogPostRepository.StoreAsync(publishedPost);
            await BlogPostRepository.StoreAsync(unpublishedPost);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddScoped<IRepository>(_ => BlogPostRepository);
            var cut = ctx.RenderComponent<DraftBlogPosts>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());

            var blogPosts = cut.FindComponents<ShortBlogPost>();

            blogPosts.Should().HaveCount(1);
            blogPosts[0].Find(".description h1").InnerHtml.Should().Be("Not published");
        }
    }
}