using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Search;
using LinkDotNet.Blog.Web.Features.Services.Tags;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Search;

public class SearchResultsViewTests : BunitContext
{
    public SearchResultsViewTests()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration
        {
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test",
        }));
        var tagQueryService = Substitute.For<ITagQueryService>();
        tagQueryService.GetPublishedOrderedByUsageAsync().Returns(Task.FromResult<IReadOnlyList<TagCount>>([]));
        Services.AddScoped(_ => tagQueryService);
    }

    [Fact]
    public void ShouldOfferTagsOfResultsAsFilterChipsWithoutScopedTag()
    {
        var cut = RenderView(
            [new BlogPostBuilder().WithTags("Scoped", "Other").Build()],
            p => p.Add(s => s.ScopedTag, "Scoped"));

        cut.FindAll(".chip").Select(c => c.TextContent.Trim()).ShouldBe(["Other1"]);
    }

    [Fact]
    public void ShouldNarrowResultsWhenChipIsSelected()
    {
        var cut = RenderView([
            new BlogPostBuilder().WithTitle("With").WithTags("A").Build(),
            new BlogPostBuilder().WithTitle("Without").WithTags("B").Build(),
        ]);

        cut.Find(".chip[aria-pressed='false']").Click();

        cut.FindComponents<ShortBlogPost>().ShouldHaveSingleItem();
        cut.Find(".result-count").TextContent.Trim().ShouldBe("1 article");
        cut.Find(".chip[aria-pressed='true']").TextContent.ShouldContain("A");
    }

    [Fact]
    public void ShouldFilterLocallyByText()
    {
        var cut = RenderView(
            [
                new BlogPostBuilder().WithTitle("Blazor").Build(),
                new BlogPostBuilder().WithTitle("Docker").Build(),
            ],
            p => p.Add(s => s.FilterTextLocally, true));

        cut.Find("input[type=search]").Input("docker");

        cut.FindComponents<ShortBlogPost>().ShouldHaveSingleItem().Instance.BlogPost.Title.ShouldBe("Docker");
    }

    [Fact]
    public void ShouldRestoreResultsWhenClearingFilters()
    {
        var cut = RenderView([
            new BlogPostBuilder().WithTags("A").Build(),
            new BlogPostBuilder().WithTags("B").Build(),
        ]);
        cut.Find(".chip").Click();

        cut.Find(".clear-filters").Click();

        cut.FindComponents<ShortBlogPost>().Count.ShouldBe(2);
        cut.FindAll(".clear-filters").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldReportDebouncedQueryChange()
    {
        string? reported = null;
        var cut = RenderView([], p => p.Add(s => s.QueryChanged, q => reported = q));

        cut.Find("input[type=search]").Input("  term ");

        cut.WaitForAssertion(() => reported.ShouldBe("term"));
    }

    [Fact]
    public void ShouldKeepTypedTextWhileParentStillPassesPreviousQuery()
    {
        var cut = RenderView([], p => p.Add(s => s.Query, "old").Add(s => s.QueryChanged, _ => { }));

        cut.Find("input[type=search]").Input("typed text");
        cut.WaitForAssertion(() => cut.Render());

        cut.Find("input[type=search]").GetAttribute("value").ShouldBe("typed text");
    }

    [Fact]
    public void ShouldKeepTrailingWhitespaceWhenParentEchoesTrimmedQuery()
    {
        var cut = RenderView([], p => p.Add(s => s.Query, "foo"));
        cut.Find("input[type=search]").Input("foo ");

        cut.Render(p => p.Add(s => s.Title, "Title").Add(s => s.Posts, []).Add(s => s.Query, "foo bar"));

        cut.Find("input[type=search]").GetAttribute("value").ShouldBe("foo bar");
    }

    [Fact]
    public void ShouldKeepResultStatusMountedAndAnnounceNoResults()
    {
        var cut = RenderView([new BlogPostBuilder().WithTitle("Blazor").Build()], p => p.Add(s => s.FilterTextLocally, true));

        cut.Find("input[type=search]").Input("nothing");

        cut.Find("[role=status]").TextContent.Trim().ShouldBe("No articles found");
    }

    [Fact]
    public void ShouldReorderResultsWhenSortingByOldest()
    {
        var cut = RenderView([
            new BlogPostBuilder().WithTitle("newer").WithUpdatedDate(new DateTime(2024, 1, 1)).Build(),
            new BlogPostBuilder().WithTitle("older").WithUpdatedDate(new DateTime(2020, 1, 1)).Build(),
        ]);

        cut.FindAll(".segmented button").Single(b => b.TextContent == "Oldest").Click();

        cut.FindComponents<ShortBlogPost>().Select(c => c.Instance.BlogPost.Title).ShouldBe(["older", "newer"]);
        cut.Find(".segmented button[aria-pressed='true']").TextContent.ShouldBe("Oldest");
    }

    [Fact]
    public void ShouldExplainHowToRecoverWhenFiltersRemoveAllResults()
    {
        var cut = RenderView(
            [new BlogPostBuilder().WithTitle("Blazor").Build()],
            p => p.Add(s => s.FilterTextLocally, true));

        cut.Find("input[type=search]").Input("nothing");

        cut.Find(".empty-hint").TextContent.ShouldContain("removing a filter");
        cut.Find(".primary-action").Click();
        cut.FindComponents<ShortBlogPost>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldShowEmptyStateWithoutPosts()
    {
        var cut = RenderView([], p => p.Add(s => s.EmptyTitle, "Nothing here"));

        cut.Find(".empty-title").TextContent.ShouldBe("Nothing here");
    }

    private IRenderedComponent<SearchResultsView> RenderView(
        IReadOnlyList<BlogPost> posts,
        Action<ComponentParameterCollectionBuilder<SearchResultsView>>? configure = null)
    {
        return Render<SearchResultsView>(p =>
        {
            p.Add(s => s.Title, "Title").Add(s => s.Posts, posts);
            configure?.Invoke(p);
        });
    }
}
