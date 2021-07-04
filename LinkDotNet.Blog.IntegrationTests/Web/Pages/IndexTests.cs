using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Pages;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages
{
    public class IndexTests : SqlDatabaseTestBase
    {
        [Fact]
        public async Task ShouldShowAllBlogPostsWithLatestOneFirst()
        {
            var oldestBlogPost = new BlogPostBuilder().WithTitle("Old").Build();
            var newestBlogPost = new BlogPostBuilder().WithTitle("New").Build();
            await BlogPostRepository.StoreAsync(oldestBlogPost);
            await BlogPostRepository.StoreAsync(newestBlogPost);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddScoped<IRepository>(_ => BlogPostRepository);
            ctx.Services.AddScoped(_ => CreateSampleAppConfiguration());
            var cut = ctx.RenderComponent<Index>();

            var blogPosts = cut.FindComponents<ShortBlogPost>();

            blogPosts.Should().HaveCount(2);
            blogPosts[0].Find(".description h1").InnerHtml.Should().Be("New");
            blogPosts[1].Find(".description h1").InnerHtml.Should().Be("Old");
        }

        private static AppConfiguration CreateSampleAppConfiguration()
        {
            return new()
            {
                Introduction = new Introduction
                {
                    Description = string.Empty,
                    BackgroundUrl = string.Empty,
                    ProfilePictureUrl = string.Empty,
                },
            };
        }
    }
}