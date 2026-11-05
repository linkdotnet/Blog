using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Search;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using Microsoft.AspNetCore.Components;
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

    [Fact]
    public async Task ShouldNotFindUnpublishedBlogPosts()
    {
        await Repository.StoreAsync(new BlogPostBuilder().WithTitle("Draft").IsPublished(false).Build());
        using var ctx = new BunitContext();
        RegisterServices(ctx);

        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Draft"));

        cut.WaitForElement(".empty-title").TextContent.ShouldBe("No results found");
        cut.FindComponents<ShortBlogPost>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldInviteToSearchWhenNoTermIsGiven()
    {
        using var ctx = new BunitContext();
        RegisterServices(ctx);

        var cut = ctx.Render<SearchPage>();

        cut.WaitForElement(".empty-title").TextContent.ShouldBe("Search the blog");
    }

    [Fact]
    public void ShouldSuggestPopularTopicsWhenNothingWasFound()
    {
        using var ctx = new BunitContext();
        RegisterServices(ctx, new TagCount("Blazor", 3));

        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "nothing"));

        var suggestion = cut.WaitForElement(".topic-suggestions a");
        suggestion.GetAttribute("href").ShouldBe("/searchByTag/Blazor");
    }

    [Fact]
    public async Task ShouldNarrowResultsWithTopicChip()
    {
        await Repository.StoreAsync(new BlogPostBuilder().WithTitle("Post one").WithTags("Cat").Build());
        await Repository.StoreAsync(new BlogPostBuilder().WithTitle("Post two").WithTags("Dog").Build());
        using var ctx = new BunitContext();
        RegisterServices(ctx);
        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Post"));
        cut.WaitForElement(".chip");

        cut.FindAll(".chip").Single(c => c.TextContent.Contains("Dog")).Click();

        cut.FindComponents<ShortBlogPost>().ShouldHaveSingleItem().Instance.BlogPost.Title.ShouldBe("Post two");
    }

    [Fact]
    public async Task ShouldNavigateToNewSearchTermWhenTypingInResultsField()
    {
        await Repository.StoreAsync(new BlogPostBuilder().WithTitle("Title 1").Build());
        using var ctx = new BunitContext();
        RegisterServices(ctx);
        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "Title"));
        cut.WaitForElement(".query-field input");

        cut.Find(".query-field input").Input("C# basics");

        var navigation = ctx.Services.GetRequiredService<NavigationManager>();
        cut.WaitForAssertion(() => navigation.Uri.ShouldEndWith("search/C%23%20basics"));
    }

    [Fact]
    public async Task ShouldIgnoreStaleResultsWhenNewerSearchWasStarted()
    {
        var slowFirstQuery = new TaskCompletionSource<IPagedList<BlogPost>>();
        var repository = Substitute.For<IRepository<BlogPost>>();
        repository.GetAllAsync(default, default).ReturnsForAnyArgs(
            _ => new ValueTask<IPagedList<BlogPost>>(slowFirstQuery.Task),
            _ => new ValueTask<IPagedList<BlogPost>>(new PagedList<BlogPost>([new BlogPostBuilder().WithTitle("Fresh").Build()], 1, 1, 1)));
        using var ctx = new BunitContext();
        RegisterServices(ctx);
        ctx.Services.AddScoped(_ => repository);
        var cut = ctx.Render<SearchPage>(p => p.Add(s => s.SearchTerm, "old"));

        cut.Render(p => p.Add(s => s.SearchTerm, "new"));
        slowFirstQuery.SetResult(new PagedList<BlogPost>([new BlogPostBuilder().WithTitle("Stale").Build()], 1, 1, 1));
        await Task.Delay(50, Xunit.TestContext.Current.CancellationToken);

        cut.WaitForAssertion(() => cut.FindComponents<ShortBlogPost>().ShouldHaveSingleItem().Instance.BlogPost.Title.ShouldBe("Fresh"));
    }

    private void RegisterServices(BunitContext ctx, params TagCount[] popularTags)
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var tagQueryService = Substitute.For<ITagQueryService>();
        tagQueryService.GetPublishedOrderedByUsageAsync().Returns(Task.FromResult<IReadOnlyList<TagCount>>(popularTags));
        ctx.Services.AddScoped(_ => tagQueryService);
    }
}
