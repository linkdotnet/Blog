using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.SupportMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class GithubSponsorTests : BunitContext
{
    [Fact]
    public void ShouldSetUrlCorrect()
    {
        var cut = Render<GithubSponsor>(
            p => p.Add(g => g.Name, "linkdotnet"));

        var anchor = cut.Find("a") as IHtmlAnchorElement;
        anchor.ShouldNotBeNull();
        anchor.Href.ShouldBe("https://github.com/sponsors/linkdotnet");
    }
}
