using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Pages;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages
{
    public class BlogPostPageTests : SqlDatabaseTestBase
    {
        [Fact]
        public async Task ShouldAddLikeOnEvent()
        {
            var publishedPost = new BlogPostBuilder().WithLikes(2).IsPublished().Build();
            await BlogPostRepository.StoreAsync(publishedPost);
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddScoped<IRepository>(_ => BlogPostRepository);
            ctx.Services.AddScoped(_ => new Mock<ILocalStorageService>().Object);
            ctx.Services.AddScoped(_ => new Mock<IToastService>().Object);
            ctx.AddTestAuthorization().SetAuthorized("s");
            var cut = ctx.RenderComponent<BlogPostPage>(
                p => p.Add(b => b.BlogPostId, publishedPost.Id));
            var likeComponent = cut.FindComponent<Like>();
            likeComponent.SetParametersAndRender(c => c.Add(p => p.BlogPost, publishedPost));

            likeComponent.Find("button").Click();

            var fromDb = await DbContext.BlogPosts.AsNoTracking().SingleAsync(d => d.Id == publishedPost.Id);
            fromDb.Likes.Should().Be(3);
        }

        [Fact]
        public async Task ShouldSubtractLikeOnEvent()
        {
            var publishedPost = new BlogPostBuilder().WithLikes(2).IsPublished().Build();
            await BlogPostRepository.StoreAsync(publishedPost);
            using var ctx = new TestContext();
            var localStorage = new Mock<ILocalStorageService>();
            localStorage.Setup(l => l.ContainKeyAsync("hasLiked", default)).ReturnsAsync(true);
            localStorage.Setup(l => l.GetItemAsync<bool>("hasLiked", default)).ReturnsAsync(true);
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddScoped<IRepository>(_ => BlogPostRepository);
            ctx.Services.AddScoped(_ => localStorage.Object);
            ctx.Services.AddScoped(_ => new Mock<IToastService>().Object);
            ctx.AddTestAuthorization().SetAuthorized("s");
            var cut = ctx.RenderComponent<BlogPostPage>(
                p => p.Add(b => b.BlogPostId, publishedPost.Id));
            var likeComponent = cut.FindComponent<Like>();
            likeComponent.SetParametersAndRender(c => c.Add(p => p.BlogPost, publishedPost));

            likeComponent.Find("button").Click();

            var fromDb = await DbContext.BlogPosts.AsNoTracking().SingleAsync(d => d.Id == publishedPost.Id);
            fromDb.Likes.Should().Be(1);
        }
    }
}