using System;
using System.Globalization;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.ShowBlogPost;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class BlogPostMarkdownRendererTests
{
    private const string CanonicalUrl = "https://localhost/blogPost/1/title";

    [Fact]
    public void ShouldRenderFrontMatterTitleAndContent()
    {
        var blogPost = new BlogPostBuilder()
            .WithTitle("My Title")
            .WithShortDescription("Short")
            .WithContent("Some **content**")
            .WithTags("C#", ".NET")
            .WithUpdatedDate(new DateTime(2026, 9, 1))
            .Build();

        var markdown = BlogPostMarkdownRenderer.Render(blogPost, [], CanonicalUrl, includeAuthor: false);

        markdown.ShouldBe("""
            ---
            title: "My Title"
            description: "Short"
            updated: 2026-09-01
            tags: ["C#", ".NET"]
            canonical: "https://localhost/blogPost/1/title"
            ---

            # My Title

            Some **content**
            """.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void ShouldOmitTagsWhenThereAreNone()
    {
        var blogPost = new BlogPostBuilder().Build();

        var markdown = BlogPostMarkdownRenderer.Render(blogPost, [], CanonicalUrl, includeAuthor: false);

        markdown.ShouldNotContain("tags:");
    }

    [Theory]
    [InlineData(true, "Jane", true)]
    [InlineData(false, "Jane", false)]
    [InlineData(true, null, false)]
    public void ShouldOnlyRenderAuthorWhenEnabledAndPresent(bool includeAuthor, string? authorName, bool expected)
    {
        var builder = new BlogPostBuilder();
        if (authorName is not null)
        {
            builder.WithAuthorName(authorName);
        }

        var markdown = BlogPostMarkdownRenderer.Render(builder.Build(), [], CanonicalUrl, includeAuthor);

        markdown.Contains("author: \"Jane\"\n", StringComparison.Ordinal).ShouldBe(expected);
        markdown.Contains("author:", StringComparison.Ordinal).ShouldBe(expected);
    }

    [Fact]
    public void ShouldEscapeFrontMatterValues()
    {
        var blogPost = new BlogPostBuilder()
            .WithTitle("Say \"Hi\": C:\\temp")
            .WithShortDescription("Line1\r\nLine2")
            .WithTags("a\"b")
            .Build();

        var markdown = BlogPostMarkdownRenderer.Render(blogPost, [], CanonicalUrl, includeAuthor: false);

        markdown.ShouldContain("title: \"Say \\\"Hi\\\": C:\\\\temp\"\n");
        markdown.ShouldContain("description: \"Line1\\nLine2\"\n");
        markdown.ShouldContain("tags: [\"a\\\"b\"]\n");
        markdown.ShouldContain("# Say \"Hi\": C:\\temp\n");
    }

    [Fact]
    public void ShouldExpandShortCodesInContent()
    {
        var blogPost = new BlogPostBuilder().WithContent("Before [[CODE]] after").Build();

        var markdown = BlogPostMarkdownRenderer.Render(blogPost, [ShortCode.Create("CODE", "**expanded**")], CanonicalUrl, includeAuthor: false);

        markdown.ShouldEndWith("Before **expanded** after");
    }

    [Fact]
    public void ShouldFormatDateIndependentOfCurrentCulture()
    {
        var blogPost = new BlogPostBuilder().WithUpdatedDate(new DateTime(2026, 9, 1)).Build();
        var previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ar-SA");

        try
        {
            var markdown = BlogPostMarkdownRenderer.Render(blogPost, [], CanonicalUrl, includeAuthor: false);

            markdown.ShouldContain("updated: 2026-09-01\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}
