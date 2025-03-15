﻿using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Search;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Search;

public class SearchPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldFindBlogPostWhenTitleMatches()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        using var ctx = new BunitContext();
        RegisterServices(ctx);

        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Title 1"));

        var blogPosts = cut.WaitForComponent<ShortBlogPost>();
        blogPosts.Find(".description h4").TextContent.ShouldBe("Title 1");
    }

    [Fact]
    public async Task ShouldFindBlogPostWhenTagMatches()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").WithTags("Cat").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").WithTags("Dog").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        using var ctx = new BunitContext();
        RegisterServices(ctx);

        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Cat"));

        var blogPost = cut.WaitForComponent<ShortBlogPost>();
        blogPost.Find(".description h4").TextContent.ShouldBe("Title 1");
    }

    [Fact]
    public async Task ShouldUnescapeQuery()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        await Repository.StoreAsync(blogPost1);
        using var ctx = new BunitContext();
        RegisterServices(ctx);

        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Title%201"));

        var blogPosts = cut.WaitForComponent<ShortBlogPost>();
        blogPosts.Find(".description h4").TextContent.ShouldBe("Title 1");
    }

    private void RegisterServices(BunitContext ctx)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IBookmarkService>());
    }
}
