using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class GithubSponsorTests : BunitContext
{
    [Fact]
    public void ShouldSetUrlCorrect()
    {
        var cut = Render<GithubSponsor>(
            p => p.Add(g => g.Name, "linkdotnet"));

        var anchor = cut.Find("a") as IHtmlAnchorElement;
        anchor.Should().NotBeNull();
        anchor.Href.Should().Be("https://github.com/sponsors/linkdotnet");
    }
}