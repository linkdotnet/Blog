using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class KofiTests : TestContext
{
    [Fact]
    public void ShouldSetToken()
    {
        var cut = RenderComponent<Kofi>(p => p.Add(s => s.KofiToken, "Token"));

        ((IHtmlAnchorElement)cut.Find("a")).Href.Should().Contain("https://ko-fi.com/Token");
    }
}