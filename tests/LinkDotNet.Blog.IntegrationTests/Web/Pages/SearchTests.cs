using System.Linq;
using System.Threading.Tasks;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Search;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages;

public class SearchTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldFindBlogPostWhenTitleMatches()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        using var ctx = new TestContext();
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IUserRecordService>());

        var cut = ctx.RenderComponent<Index>(p => p.Add(s => s.SearchTerm, "Title 1"));

        cut.WaitForState(() => cut.FindComponents<ShortBlogPost>().Any());
        var blogPosts = cut.FindComponents<ShortBlogPost>();
        blogPosts.Should().HaveCount(1);
        blogPosts.Single().Find(".description h1").TextContent.Should().Be("Title 1");
    }

    [Fact]
    public async Task ShouldFindBlogPostWhenTagMatches()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").WithTags("Cat").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").WithTags("Dog").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        using var ctx = new TestContext();
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IUserRecordService>());

        var cut = ctx.RenderComponent<Index>(p => p.Add(s => s.SearchTerm, "Cat"));

        cut.WaitForState(() => cut.FindComponents<ShortBlogPost>().Any());
        var blogPosts = cut.FindComponents<ShortBlogPost>();
        blogPosts.Should().HaveCount(1);
        blogPosts.Single().Find(".description h1").TextContent.Should().Be("Title 1");
    }

    [Fact]
    public async Task ShouldUnescapeQuery()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        await Repository.StoreAsync(blogPost1);
        using var ctx = new TestContext();
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IUserRecordService>());

        var cut = ctx.RenderComponent<Index>(p => p.Add(s => s.SearchTerm, "Title%201"));

        cut.WaitForState(() => cut.FindComponents<ShortBlogPost>().Any());
        var blogPosts = cut.FindComponents<ShortBlogPost>();
        blogPosts.Should().HaveCount(1);
        blogPosts.Single().Find(".description h1").TextContent.Should().Be("Title 1");
    }
}