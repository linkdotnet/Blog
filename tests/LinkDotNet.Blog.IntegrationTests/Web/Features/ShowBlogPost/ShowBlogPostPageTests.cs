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
using Microsoft.Extensions.Options;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.ShowBlogPost;

public class ShowBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldAddLikeOnEvent()
    {
        var publishedPost = new BlogPostBuilder().WithLikes(2).IsPublished().Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);
        ctx.AddAuthorization().SetAuthorized("s");
        var cut = ctx.Render<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));
        var likeComponent = cut.FindComponent<Like>();
        likeComponent.Render(c => c.Add(p => p.BlogPost, publishedPost));

        likeComponent.Find("span").Click();

        var fromDb = await DbContext.BlogPosts.AsNoTracking().SingleAsync(d => d.Id == publishedPost.Id);
        fromDb.Likes.ShouldBe(3);
    }

    [Fact]
    public async Task ShouldSubtractLikeOnEvent()
    {
        var publishedPost = new BlogPostBuilder().WithLikes(2).IsPublished().Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        var localStorage = Substitute.For<ILocalStorageService>();
        var hasLikedStorage = $"hasLiked/{publishedPost.Id}";
        localStorage.ContainKeyAsync(hasLikedStorage).Returns(true);
        localStorage.GetItemAsync<bool>(hasLikedStorage).Returns(true);
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx, localStorage);
        ctx.AddAuthorization().SetAuthorized("s");
        var cut = ctx.Render<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));
        var likeComponent = cut.FindComponent<Like>();
        likeComponent.Render(c => c.Add(p => p.BlogPost, publishedPost));

        likeComponent.Find("span").Click();

        var fromDb = await DbContext.BlogPosts.AsNoTracking().SingleAsync(d => d.Id == publishedPost.Id);
        fromDb.Likes.ShouldBe(1);
    }

    [Fact]
    public async Task ShouldSetTagsWhenAvailable()
    {
        var publishedPost = new BlogPostBuilder().IsPublished().WithTags("Tag1,Tag2").Build();
        await Repository.StoreAsync(publishedPost);
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.AddAuthorization();
        RegisterComponents(ctx);
        var cut = ctx.Render<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, publishedPost.Id));

        var ogData = cut.FindComponent<OgData>();

        ogData.Instance.Keywords.ShouldBe("Tag1,Tag2");
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
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.AddAuthorization();
        RegisterComponents(ctx);
        ctx.ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));
        var cut = ctx.Render<ShowBlogPostPage>(
            p => p.Add(b => b.BlogPostId, post.Id));

        var structuredData = cut.FindComponent<StructuredData>();

        structuredData.Instance.Headline.ShouldBe("Title");
        structuredData.Instance.PreviewImage.ShouldBe("image1");
        structuredData.Instance.PreviewFallbackImage.ShouldBe("image2");
    }

    [Fact]
    public void ShouldShowErrorPageWhenBlogPostNotFound()
    {
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        RegisterComponents(ctx);

        var cut = ctx.Render<ShowBlogPostPage>();

        cut.FindAll(".blogpost-content").ShouldBeEmpty();
        cut.FindAll("#no-blog-post-error").ShouldHaveSingleItem();
    }

    private void RegisterComponents(BunitContext ctx, ILocalStorageService localStorageService = null)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => localStorageService ?? Substitute.For<ILocalStorageService>());
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        ctx.Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        ctx.Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        ctx.Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
    }
}