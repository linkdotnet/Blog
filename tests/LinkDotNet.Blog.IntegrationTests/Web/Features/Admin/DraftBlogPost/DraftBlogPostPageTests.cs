﻿using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.DraftBlogPost;
 using LinkDotNet.Blog.Web.Features.Bookmarks;
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
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var cut = ctx.Render<DraftBlogPostPage>();
        cut.WaitForElement(".blog-card");

        var blogPosts = cut.FindComponents<ShortBlogPost>();

        blogPosts.ShouldHaveSingleItem();
        blogPosts[0].Find(".description h4").InnerHtml.ShouldBe("Not published");
    }
}
