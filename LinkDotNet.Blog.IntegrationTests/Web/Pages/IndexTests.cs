using System.Linq;
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
using Moq;
using Toolbelt.Blazor.HeadElement;
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
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());

            var blogPosts = cut.FindComponents<ShortBlogPost>();

            blogPosts.Should().HaveCount(2);
            blogPosts[0].Find(".description h1").InnerHtml.Should().Be("New");
            blogPosts[1].Find(".description h1").InnerHtml.Should().Be("Old");
        }

        [Fact]
        public async Task ShouldOnlyShowPublishedPosts()
        {
            var publishedPost = new BlogPostBuilder().WithTitle("Published").IsPublished().Build();
            var unpublishedPost = new BlogPostBuilder().WithTitle("Not published").IsPublished(false).Build();
            await BlogPostRepository.StoreAsync(publishedPost);
            await BlogPostRepository.StoreAsync(unpublishedPost);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());

            var blogPosts = cut.FindComponents<ShortBlogPost>();

            blogPosts.Should().HaveCount(1);
            blogPosts[0].Find(".description h1").InnerHtml.Should().Be("Published");
        }

        [Fact]
        public async Task ShouldOnlyLoadTenEntities()
        {
            await CreatePublishedBlogPosts(11);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());

            var blogPosts = cut.FindComponents<ShortBlogPost>();

            blogPosts.Count.Should().Be(10);
        }

        [Fact]
        public async Task ShouldLoadNextBatchOnClick()
        {
            await CreatePublishedBlogPosts(11);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();

            cut.FindComponent<BlogPostNavigation>().Find("li:last-child a").Click();

            cut.WaitForState(() => cut.FindAll(".blog-card").Count == 1);
            var blogPosts = cut.FindComponents<ShortBlogPost>();
            blogPosts.Count.Should().Be(1);
        }

        [Fact]
        public async Task ShouldLoadPreviousBatchOnClick()
        {
            await CreatePublishedBlogPosts(11);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());
            cut.FindComponent<BlogPostNavigation>().Find("li:last-child a").Click();
            cut.WaitForState(() => cut.FindAll(".blog-card").Count == 1);

            cut.FindComponent<BlogPostNavigation>().Find("li:first-child a").Click();

            cut.WaitForState(() => cut.FindAll(".blog-card").Count > 1);
            var blogPosts = cut.FindComponents<ShortBlogPost>();
            blogPosts.Count.Should().Be(10);
        }

        private static AppConfiguration CreateSampleAppConfiguration()
        {
            return new()
            {
                BlogName = string.Empty,
                Introduction = new Introduction
                {
                    Description = string.Empty,
                    BackgroundUrl = string.Empty,
                    ProfilePictureUrl = string.Empty,
                },
                BlogPostsPerPage = 10,
            };
        }

        private async Task CreatePublishedBlogPosts(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var blogPost = new BlogPostBuilder().IsPublished().Build();
                await BlogPostRepository.StoreAsync(blogPost);
            }
        }

        private void RegisterComponents(TestContextBase ctx)
        {
            ctx.Services.AddScoped<IRepository>(_ => BlogPostRepository);
            ctx.Services.AddScoped(_ => CreateSampleAppConfiguration());
            ctx.Services.AddScoped(_ => new Mock<IHeadElementHelper>().Object);
        }
    }
}