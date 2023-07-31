using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.ShowBlogPost;

public class ShowBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
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
        var cut = ctx.RenderComponent<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));
        var likeComponent = cut.FindComponent<Like>();
        likeComponent.SetParametersAndRender(c => c.Add(p => p.BlogPost, publishedPost));

        likeComponent.Find("span").Click();

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
        var cut = ctx.RenderComponent<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));
        var likeComponent = cut.FindComponent<Like>();
        likeComponent.SetParametersAndRender(c => c.Add(p => p.BlogPost, publishedPost));

        likeComponent.Find("span").Click();

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
        var cut = ctx.RenderComponent<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));

        var ogData = cut.FindComponent<OgData>();

        ogData.Instance.Keywords.Should().Be("Tag1,Tag2");
    }

    [Fact]
    public async Task ShouldSetStructuredData()
    {
        var post = new BlogPostBuilder()
            .WithTitle("Title")
            .WithPreviewImageUrl("image1")
            .WithPreviewImageUrlFallback("image2")
            .Build();
        await Repository.StoreAsync(post);
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.AddTestAuthorization();
        RegisterComponents(ctx);
        ctx.ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));
        var cut = ctx.RenderComponent<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, post.Id));

        var structuredData = cut.FindComponent<StructuredData>();

        structuredData.Instance.Headline.Should().Be("Title");
        structuredData.Instance.PreviewImage.Should().Be("image1");
        structuredData.Instance.PreviewFallbackImage.Should().Be("image2");
    }

    private void RegisterComponents(TestContextBase ctx, ILocalStorageService localStorageService = null)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => localStorageService ?? Mock.Of<ILocalStorageService>());
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        ctx.Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        ctx.Services.AddScoped(_ => new AppConfiguration());
    }
}