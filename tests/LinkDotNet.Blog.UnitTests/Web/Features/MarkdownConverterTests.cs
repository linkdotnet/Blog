using LinkDotNet.Blog.Web.Features;

namespace LinkDotNet.Blog.UnitTests.Web.Features;

public class MarkdownConverterTests
{
    [Theory]
    [InlineData("NOTE", "Note", "alert-primary", "bi-info-circle")]
    [InlineData("tip", "Tip", "alert-success", "bi-lightbulb")]
    [InlineData("IMPORTANT", "Important", "alert-info", "bi-exclamation-square")]
    [InlineData("WARNING", "Warning", "alert-warning", "bi-exclamation-triangle")]
    [InlineData("CAUTION", "Caution", "alert-danger", "bi-exclamation-octagon")]
    public void ShouldRenderCalloutWithTitle(string kind, string title, string cssClass, string icon)
    {
        var markdown = $"> [!{kind}]\n> Some **content**";

        var html = MarkdownConverter.ToMarkupString(markdown).Value;

        html.ShouldContain($"markdown-alert-{kind.ToLowerInvariant()}");
        html.ShouldContain(cssClass);
        html.ShouldContain($"""<i class="bi {icon}" aria-hidden="true"></i>{title}</p>""");
        html.ShouldContain("<strong>content</strong>");
    }

    [Fact]
    public void ShouldRenderRegularQuoteAsBlockquote()
    {
        var html = MarkdownConverter.ToMarkupString("> Just a quote").Value;

        html.ShouldContain("<blockquote");
        html.ShouldNotContain("markdown-alert");
    }

    [Fact]
    public void ShouldAddHeadingAnchorWithFullUrl()
    {
        var html = MarkdownConverter.ToMarkupStringWithHeadingAnchors(
            "## Hello World",
            "https://localhost/blogPost/1/slug").Value;

        html.ShouldContain("""<h2 id="hello-world">Hello World<a href="https://localhost/blogPost/1/slug#hello-world" class="heading-anchor """);
        html.ShouldContain("""aria-label="Link to this section">#</a></h2>""");
    }

    [Fact]
    public void ShouldReplaceExistingFragmentInHeadingAnchor()
    {
        var html = MarkdownConverter.ToMarkupStringWithHeadingAnchors(
            "# First\n\n## Second",
            "https://localhost/blogPost/1#first").Value;

        html.ShouldContain("href=\"https://localhost/blogPost/1#first\"");
        html.ShouldContain("href=\"https://localhost/blogPost/1#second\"");
        html.ShouldNotContain("#first#");
    }

    [Fact]
    public void ShouldNotAddHeadingAnchorsToRegularMarkup()
    {
        var html = MarkdownConverter.ToMarkupString("## Hello World").Value;

        html.ShouldNotContain("heading-anchor");
    }

    [Fact]
    public void ShouldNotAddHeadingAnchorToTableOfContents()
    {
        var toc = MarkdownConverter.GenerateToc("## Hello World");

        toc.ShouldHaveSingleItem().Text.ShouldBe("Hello World");
    }

    [Fact]
    public void ShouldReturnEmptyMarkupForEmptyContentWithHeadingAnchors()
    {
        MarkdownConverter.ToMarkupStringWithHeadingAnchors(string.Empty, "https://localhost").Value.ShouldBeNull();
    }

    [Fact]
    public void ShouldLazyLoadImages()
    {
        var html = MarkdownConverter.ToMarkupString("![alt](https://localhost/image.png)").Value;

        html.ShouldBe("""<p><img src="https://localhost/image.png" class="img-fluid" loading="lazy" alt="alt" /></p>""" + "\n");
    }

    [Fact]
    public void ShouldNotAddLazyLoadingToLinks()
    {
        var html = MarkdownConverter.ToMarkupString("[link](https://localhost)").Value;

        html.ShouldNotContain("loading=");
    }
}
