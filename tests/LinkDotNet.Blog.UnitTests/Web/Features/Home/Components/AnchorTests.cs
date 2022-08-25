using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class AnchorTests : TestContext
{
    [Fact]
    public void ShouldShowHrefWhenNotEmpty()
    {
        var cut = RenderComponent<Anchor>(ps => ps
            .Add(p => p.Href, "http://url/")
            .Add(p => p.CssClass, "page"));

        var anchor = cut.Find("a").Unwrap() as IHtmlAnchorElement;
        anchor.Should().NotBeNull();
        anchor.Href.Should().Be("http://url/");
        anchor.GetAttribute("class").Should().Be("page");
    }

    [Fact]
    public void ShouldNotShowHrefWhenEmpty()
    {
        var cut = RenderComponent<Anchor>(ps => ps
            .Add(p => p.Href, string.Empty)
            .Add(p => p.CssClass, "page"));

        var anchor = cut.Find("a").Unwrap() as IHtmlAnchorElement;
        anchor.Should().NotBeNull();
        anchor.Attributes.Count(a => a.Name == "href").Should().Be(0);
    }
}