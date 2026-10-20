using LinkDotNet.Blog.Web.Features.ShowBlogPost;
using Microsoft.Net.Http.Headers;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class BlogPostMarkdownMiddlewareTests
{
    [Theory]
    [InlineData("text/markdown", true)]
    [InlineData("TEXT/MARKDOWN", true)]
    [InlineData("text/markdown; charset=utf-8", true)]
    [InlineData("text/markdown, text/html;q=0.9", true)]
    [InlineData("text/html;q=0.5, text/markdown", true)]
    [InlineData("text/markdown, */*", true)]
    [InlineData("text/markdown;q=0.8, text/*;q=0.8", true)]
    [InlineData("text/markdown;q=0.8, text/html;q=0, */*;q=1", true)]
    [InlineData("text/markdown;q=0.5, text/*;q=0, */*", true)]
    [InlineData("text/markdown;q=0.5, text/*, */*;q=0", false)]
    [InlineData("text/markdown;q=0", false)]
    [InlineData("text/html, text/markdown;q=0.9", false)]
    [InlineData("text/plain, text/markdown;q=0.9, */*", false)]
    [InlineData("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8", false)]
    [InlineData("*/*", false)]
    [InlineData("text/*", false)]
    [InlineData("text/plain", false)]
    [InlineData("", false)]
    public void ShouldPreferMarkdownOnlyWhenRankedAtLeastAsHighAsHtml(string accept, bool expected)
    {
        var mediaTypes = MediaTypeHeaderValue.ParseList([accept]);

        BlogPostMarkdownMiddleware.PrefersMarkdown(mediaTypes).ShouldBe(expected);
    }
}
