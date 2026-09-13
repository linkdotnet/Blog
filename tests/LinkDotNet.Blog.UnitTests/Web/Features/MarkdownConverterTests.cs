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
    public void HeadingAnchorShouldCopyLinkToClipboard()
    {
        var html = MarkdownConverter.ToMarkupStringWithHeadingAnchors("## Hello World", "https://localhost/blogPost/1").Value;

        html.ShouldContain("""title="Copy link to this section" onclick="navigator.clipboard?.writeText(this.href)""");
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
    public void ShouldGenerateTocTextForHeadingWithExternalLink()
    {
        var toc = MarkdownConverter.GenerateToc("## [Markdig](https://github.com/xoofx/markdig)");

        toc.ShouldHaveSingleItem().Text.ShouldBe("Markdig");
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

    [Fact]
    public void ShouldMarkAbsoluteLinksAsExternal()
    {
        var html = MarkdownConverter.ToMarkupString("[link](https://localhost)").Value;

        html.ShouldContain("target=\"_blank\"");
        html.ShouldContain("rel=\"noopener noreferrer\"");
        html.ShouldContain("class=\"external-link\"");
        html.ShouldContain("""<i class="bi bi-box-arrow-up-right ms-1" aria-hidden="true"></i>""");
    }

    [Fact]
    public void ShouldNotMarkRelativeLinksAsExternal()
    {
        var html = MarkdownConverter.ToMarkupString("[post](/blogPost/1)").Value;

        html.ShouldNotContain("external-link");
        html.ShouldNotContain("target=");
    }

    [Fact]
    public void ShouldNotMarkFragmentLinksAsExternal()
    {
        var html = MarkdownConverter.ToMarkupString("[jump](#section)").Value;

        html.ShouldNotContain("external-link");
        html.ShouldNotContain("target=");
    }

    [Fact]
    public void ShouldNotMarkExternalImagesAsExternalLinks()
    {
        var html = MarkdownConverter.ToMarkupString("![alt](https://localhost/image.png)").Value;

        html.ShouldNotContain("external-link");
        html.ShouldNotContain("bi-box-arrow-up-right");
    }

    [Fact]
    public void ShouldRenderLanguageBadgeForFencedCodeBlock()
    {
        var html = MarkdownConverter.ToMarkupString("```csharp\nvar x = 1;\n```").Value;

        html.ShouldContain("""<span class="badge bg-secondary position-absolute top-0 start-0 m-2 code-lang-badge">csharp</span>""");
    }

    [Fact]
    public void ShouldNotRenderLanguageBadgeWithoutLanguage()
    {
        var html = MarkdownConverter.ToMarkupString("```\nvar x = 1;\n```").Value;

        html.ShouldNotContain("code-lang-badge");
    }

    [Fact]
    public void ShouldNotRenderLanguageBadgeForIndentedCodeBlock()
    {
        var html = MarkdownConverter.ToMarkupString("    var x = 1;").Value;

        html.ShouldNotContain("code-lang-badge");
    }

    [Fact]
    public void ShouldRenderTaskListCheckboxes()
    {
        var html = MarkdownConverter.ToMarkupString("- [ ] Todo item\n- [x] Done item\n- Regular item").Value;

        html.ShouldContain("contains-task-list");
        html.ShouldContain("""<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Todo item</li>""");
        html.ShouldContain("""<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Done item</li>""");
        html.ShouldContain("<li>Regular item</li>");
    }

    [Fact]
    public void ShouldWrapTablesInResponsiveContainer()
    {
        var html = MarkdownConverter.ToMarkupString("| A | B |\n|---|---|\n| 1 | 2 |").Value;

        html.ShouldContain("""<div class="table-container">""");
        html.ShouldContain("""<table class="table">""");
        html.ShouldContain("</table>\n</div>");
    }
}
