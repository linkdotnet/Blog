using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class GithubSponsorTests : TestContext
{
    [Fact]
    public void ShouldSetUrlCorrect()
    {
        var cut = RenderComponent<GithubSponsor>(
            p => p.Add(g => g.Name, "linkdotnet"));

        var anchor = cut.Find("a").Unwrap() as IHtmlAnchorElement;
        anchor.Should().NotBeNull();
        anchor.Href.Should().Be("https://github.com/sponsors/linkdotnet");
    }
}