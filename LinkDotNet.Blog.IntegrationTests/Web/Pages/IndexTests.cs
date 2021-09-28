using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.HeadElement;
using Xunit;

using Index = LinkDotNet.Blog.Web.Pages.Index;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages
{
    public class IndexTests : SqlDatabaseTestBase<BlogPost>
    {
        [Fact]
        public async Task ShouldShowAllBlogPostsWithLatestOneFirst()
        {
            var oldestBlogPost = new BlogPostBuilder().WithTitle("Old").Build();
            var newestBlogPost = new BlogPostBuilder().WithTitle("New").Build();
            await Repository.StoreAsync(oldestBlogPost);
            await Repository.StoreAsync(newestBlogPost);
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
            await Repository.StoreAsync(publishedPost);
            await Repository.StoreAsync(unpublishedPost);
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
        public async Task ShouldLoadOnlyItemsInPage()
        {
            await CreatePublishedBlogPosts(11);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);

            var cut = ctx.RenderComponent<Index>(
                p => p.Add(s => s.Page, 2));

            cut.WaitForState(() => cut.FindAll(".blog-card").Any());
            var blogPosts = cut.FindComponents<ShortBlogPost>();
            blogPosts.Should().HaveCount(1);
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
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);
            var cut = ctx.RenderComponent<Index>();
            cut.WaitForState(() => cut.FindAll(".blog-card").Any());

            var tags = cut.FindComponent<ShortBlogPost>().FindAll(".goto-tag");

            tags.Should().HaveCount(2);
            tags.Select(t => t.TextContent).Should().Contain("C Sharp");
            tags.Select(t => t.TextContent).Should().Contain("Tag2");
            tags.Select(t => t.Attributes.Single(a => a.Name == "href").Value).Should().Contain("/searchByTag/C%20Sharp");
            tags.Select(t => t.Attributes.Single(a => a.Name == "href").Value).Should().Contain("/searchByTag/Tag2");
        }

        [Theory]
        [InlineData("relative/url", "http://localhost/relative/url")]
        [InlineData("http://localhost/relative/url", "http://localhost/relative/url")]
        public void ShouldSetAbsoluteUriForOgData(string givenUri, string expectedUri)
        {
            using var ctx = new TestContext();
            RegisterComponents(ctx);
            ctx.Services.GetService<AppConfiguration>()!.Introduction.BackgroundUrl = givenUri;

            var cut = ctx.RenderComponent<Index>();

            cut.WaitForState(() => cut.FindComponents<OgData>().Any());
            cut.FindComponent<OgData>().Instance.AbsolutePreviewImageUrl.Should().Be(expectedUri);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ShouldSetPageToFirstIfOutOfRange(int? page)
        {
            await CreatePublishedBlogPosts(11);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            RegisterComponents(ctx);

            var cut = ctx.RenderComponent<Index>(p => p.Add(
                i => i.Page, page));

            cut.WaitForState(() => cut.FindAll(".blog-card").Any());
            cut.WaitForState(() => cut.FindAll(".blog-card").Count == 10);
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
                await Repository.StoreAsync(blogPost);
            }
        }

        private void RegisterComponents(TestContextBase ctx)
        {
            ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
            ctx.Services.AddScoped(_ => CreateSampleAppConfiguration());
            ctx.Services.AddScoped(_ => new Mock<IHeadElementHelper>().Object);
            ctx.Services.AddScoped(_ => new Mock<IUserRecordService>().Object);
        }
    }
}