using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.SearchByTag;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.SearchByTag;

public class SearchByTagTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldOnlyDisplayTagsGivenByParameter()
    {
        using var ctx = new BunitContext();
        await AddBlogPostWithTagAsync("Tag 1");
        await AddBlogPostWithTagAsync("Tag 1");
        await AddBlogPostWithTagAsync("Tag 1", isPublished: false);
        await AddBlogPostWithTagAsync("Tag 2");
        RegisterServices(ctx);
        var cut = ctx.Render<SearchByTagPage>(p => p.Add(s => s.Tag, "Tag 1"));
        cut.WaitForElement(".blog-card");

        var tags = cut.FindAll(".blog-card");

        tags.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ShouldHandleSpecialCharacters()
    {
        using var ctx = new BunitContext();
        await AddBlogPostWithTagAsync("C#");
        RegisterServices(ctx);
        var cut = ctx.Render<SearchByTagPage>(p => p.Add(s => s.Tag, Uri.EscapeDataString("C#")));
        cut.WaitForElement(".blog-card");

        var tags = cut.FindAll(".blog-card");

        tags.ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldSetTitleToTag()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped(_ => Repository);
        ctx.ComponentFactories.Add<PageTitle, PageTitleStub>();

        var cut = ctx.Render<SearchByTagPage>(p => p.Add(s => s.Tag, "Tag"));

        var pageTitleStub = cut.FindComponent<PageTitleStub>();
        var pageTitle = ctx.Render(pageTitleStub.Instance.ChildContent!);
        pageTitle.Markup.ShouldBe("Search for tag: Tag");
    }

    private async Task AddBlogPostWithTagAsync(string tag, bool isPublished = true)
    {
        var blogPost = new BlogPostBuilder().WithTags(tag).IsPublished(isPublished).Build();
        await DbContext.AddAsync(blogPost);
        await DbContext.SaveChangesAsync();
    }

    private void RegisterServices(BunitContext ctx)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IBookmarkService>());
    }
    
    private class PageTitleStub : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; } = default!;
    }
}
