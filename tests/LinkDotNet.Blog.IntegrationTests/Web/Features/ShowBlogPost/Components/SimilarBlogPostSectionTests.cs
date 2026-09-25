using System.Collections.Generic;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.ShowBlogPost.Components;

public class SimilarBlogPostSectionTests
{
    [Fact]
    public void ShouldShowSimilarBlogPosts()
    {
        using var context = new BunitContext();
        IReadOnlyList<BlogPostSummary> similarBlogPosts = [CreateSummary("Title 2"), CreateSummary("Title 3")];

        var cut = context.Render<SimilarBlogPostSection>(p => p.Add(s => s.SimilarBlogPosts, similarBlogPosts));

        var elements = cut.FindAll("h6");
        elements.Count.ShouldBe(2);
        elements.ShouldContain(p => p.TextContent == "Title 2");
        elements.ShouldContain(p => p.TextContent == "Title 3");
        cut.Find("a").GetAttribute("href").ShouldBe("blogPost/Title 2/title-2");
    }

    [Fact]
    public void ShouldRenderNothingWithoutSimilarBlogPosts()
    {
        using var context = new BunitContext();

        var cut = context.Render<SimilarBlogPostSection>(p => p.Add(s => s.SimilarBlogPosts, []));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    private static BlogPostSummary CreateSummary(string title) => new()
    {
        Id = title,
        Title = title,
        ShortDescription = "Short",
        PreviewImageUrl = "url",
        IsPublished = true,
    };
}
