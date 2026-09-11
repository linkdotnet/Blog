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
}
