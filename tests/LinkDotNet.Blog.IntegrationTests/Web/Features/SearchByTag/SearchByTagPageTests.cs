using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.SearchByTag;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.SearchByTag;

public class SearchByTagTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldOnlyDisplayTagsGivenByParameter()
    {
        using var ctx = new TestContext();
        await AddBlogPostWithTagAsync("Tag 1");
        await AddBlogPostWithTagAsync("Tag 1");
        await AddBlogPostWithTagAsync("Tag 1", isPublished: false);
        await AddBlogPostWithTagAsync("Tag 2");
        RegisterServices(ctx);
        var cut = ctx.RenderComponent<SearchByTagPage>(p => p.Add(s => s.Tag, "Tag 1"));
        cut.WaitForState(() => cut.FindAll(".blog-card").Any());

        var tags = cut.FindAll(".blog-card");

        tags.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldHandleSpecialCharacters()
    {
        using var ctx = new TestContext();
        await AddBlogPostWithTagAsync("C#");
        RegisterServices(ctx);
        var cut = ctx.RenderComponent<SearchByTagPage>(p => p.Add(s => s.Tag, Uri.EscapeDataString("C#")));
        cut.WaitForState(() => cut.FindAll(".blog-card").Any());

        var tags = cut.FindAll(".blog-card");

        tags.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldSetTitleToTag()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        ctx.ComponentFactories.AddStub<PageTitle>();

        var cut = ctx.RenderComponent<SearchByTagPage>(p => p.Add(s => s.Tag, "Tag"));

        var pageTitleStub = cut.FindComponent<PageTitleStub>();
        var pageTitle = ctx.Render(pageTitleStub.Instance.ChildContent!);
        pageTitle.Markup.Should().Be("Search for tag: Tag");
    }

    private async Task AddBlogPostWithTagAsync(string tag, bool isPublished = true)
    {
        var blogPost = new BlogPostBuilder().WithTags(tag).IsPublished(isPublished).Build();
        await DbContext.AddAsync(blogPost);
        await DbContext.SaveChangesAsync();
    }

    private void RegisterServices(TestContext ctx)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        ctx.Services.AddMemoryCache();
    }
}
