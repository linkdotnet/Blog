using System.Threading.Tasks;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Pages;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.HeadElement;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages;

public class BlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldAddLikeOnEvent()
    {
        var publishedPost = new BlogPostBuilder().WithLikes(2).IsPublished().Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
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
        await Repository.StoreAsync(publishedPost);
        using var ctx = new TestContext();
        var localStorage = new Mock<ILocalStorageService>();
        var hasLikedStorage = $"hasLiked/{publishedPost.Id}";
        localStorage.Setup(l => l.ContainKeyAsync(hasLikedStorage)).ReturnsAsync(true);
        localStorage.Setup(l => l.GetItemAsync<bool>(hasLikedStorage)).ReturnsAsync(true);
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx, localStorage.Object);
        ctx.AddTestAuthorization().SetAuthorized("s");
        var cut = ctx.RenderComponent<BlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));
        var likeComponent = cut.FindComponent<Like>();
        likeComponent.SetParametersAndRender(c => c.Add(p => p.BlogPost, publishedPost));

        likeComponent.Find("button").Click();

        var fromDb = await DbContext.BlogPosts.AsNoTracking().SingleAsync(d => d.Id == publishedPost.Id);
        fromDb.Likes.Should().Be(1);
    }

    [Fact]
    public async Task ShouldSetTagsWhenAvailable()
    {
        var publishedPost = new BlogPostBuilder().IsPublished().WithTags("Tag1,Tag2").Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.AddTestAuthorization();
        RegisterComponents(ctx);
        var cut = ctx.RenderComponent<BlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));

        var ogData = cut.FindComponent<OgData>();

        ogData.Instance.Keywords.Should().Be("Tag1,Tag2");
    }

    private void RegisterComponents(TestContextBase ctx, ILocalStorageService localStorageService = null)
    {
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
        ctx.Services.AddScoped(_ => localStorageService ?? new Mock<ILocalStorageService>().Object);
        ctx.Services.AddScoped(_ => new Mock<IToastService>().Object);
        ctx.Services.AddScoped(_ => new Mock<IHeadElementHelper>().Object);
        ctx.Services.AddScoped(_ => new Mock<IUserRecordService>().Object);
        ctx.Services.AddScoped(_ => new AppConfiguration());
    }
}
