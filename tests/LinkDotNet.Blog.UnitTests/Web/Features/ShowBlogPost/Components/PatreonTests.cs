using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class PatreonTests : TestContext
{
    [Fact]
    public void ShouldSetUrlCorrect()
    {
        var cut = RenderComponent<Patreon>(
            p => p.Add(s => s.PatreonName, "linkdotnet"));

        var anchor = cut.Find("a") as IHtmlAnchorElement;

        anchor.Should().NotBeNull();
        anchor.Href.Should().Be("https://www.patreon.com/linkdotnet");
    }
}
