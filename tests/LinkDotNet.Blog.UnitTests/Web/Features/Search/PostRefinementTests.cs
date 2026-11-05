using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Search;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Search;

public class PostRefinementTests
{
    private static readonly IReadOnlySet<string> NoTags = new HashSet<string>();

    [Fact]
    public void ShouldCountTagsByUsageAndExcludeScopedTag()
    {
        var posts = new[]
        {
            new BlogPostBuilder().WithTags("A", "B").Build(),
            new BlogPostBuilder().WithTags("A", "C").Build(),
            new BlogPostBuilder().WithTags("A").Build(),
        };

        var tags = PostRefinement.CountTags(posts, excludedTag: "A");

        tags.Select(t => t.Name).ShouldBe(["B", "C"]);
        tags.ShouldAllBe(t => t.Count == 1);
    }

    [Fact]
    public void ShouldRequireAllSelectedTags()
    {
        var both = new BlogPostBuilder().WithTitle("both").WithTags("A", "B").Build();
        var onlyA = new BlogPostBuilder().WithTitle("onlyA").WithTags("A").Build();

        var result = PostRefinement.Apply([both, onlyA], new HashSet<string> { "A", "B" }, string.Empty, PostSortOrder.Newest);

        result.ShouldHaveSingleItem().Title.ShouldBe("both");
    }

    [Fact]
    public void ShouldFilterByTextIgnoringCase()
    {
        var match = new BlogPostBuilder().WithTitle("Blazor Basics").Build();
        var other = new BlogPostBuilder().WithTitle("Docker").WithShortDescription("containers").Build();

        var result = PostRefinement.Apply([match, other], NoTags, "  blazor ", PostSortOrder.Newest);

        result.ShouldHaveSingleItem().Title.ShouldBe("Blazor Basics");
    }

    [Fact]
    public void ShouldSortOldestFirst()
    {
        var older = new BlogPostBuilder().WithTitle("older").WithUpdatedDate(new DateTime(2020, 1, 1)).Build();
        var newer = new BlogPostBuilder().WithTitle("newer").WithUpdatedDate(new DateTime(2024, 1, 1)).Build();

        var result = PostRefinement.Apply([newer, older], NoTags, string.Empty, PostSortOrder.Oldest);

        result.Select(p => p.Title).ShouldBe(["older", "newer"]);
    }

    [Fact]
    public void ShouldSortNewestFirst()
    {
        var older = new BlogPostBuilder().WithTitle("older").WithUpdatedDate(new DateTime(2020, 1, 1)).Build();
        var newer = new BlogPostBuilder().WithTitle("newer").WithUpdatedDate(new DateTime(2024, 1, 1)).Build();

        var result = PostRefinement.Apply([older, newer], NoTags, string.Empty, PostSortOrder.Newest);

        result.Select(p => p.Title).ShouldBe(["newer", "older"]);
    }

    [Fact]
    public void ShouldSortShortestReadFirst()
    {
        var long1 = new BlogPostBuilder().WithTitle("long").WithContent(string.Join(' ', Enumerable.Repeat("word", 5000))).Build();
        var short1 = new BlogPostBuilder().WithTitle("short").WithContent("word").Build();

        var result = PostRefinement.Apply([long1, short1], NoTags, string.Empty, PostSortOrder.ShortestRead);

        result.Select(p => p.Title).ShouldBe(["short", "long"]);
    }
}
