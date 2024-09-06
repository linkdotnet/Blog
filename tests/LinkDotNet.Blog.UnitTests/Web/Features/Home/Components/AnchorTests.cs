using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class AnchorTests : BunitContext
{
    [Fact]
    public void ShouldShowHrefWhenNotEmpty()
    {
        var cut = Render<Anchor>(ps => ps
            .Add(p => p.Href, "http://url/")
            .Add(p => p.CssClass, "page"));

        var anchor = cut.Find("a") as IHtmlAnchorElement;
        anchor.ShouldNotBeNull();
        anchor.Href.ShouldBe("http://url/");
        anchor.GetAttribute("class").ShouldBe("page");
    }

    [Fact]
    public void ShouldNotShowHrefWhenEmpty()
    {
        var cut = Render<Anchor>(ps => ps
            .Add(p => p.Href, string.Empty)
            .Add(p => p.CssClass, "page"));

        var anchor = cut.Find("a") as IHtmlAnchorElement;
        anchor.ShouldNotBeNull();
        anchor.Attributes.Any(a => a.Name == "href").ShouldBeFalse();
    }
}