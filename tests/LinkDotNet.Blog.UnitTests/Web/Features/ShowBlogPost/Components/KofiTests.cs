using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.SupportMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class KofiTests : BunitContext
{
    [Fact]
    public void ShouldSetToken()
    {
        var cut = Render<Kofi>(p => p.Add(s => s.KofiToken, "Token"));

        ((IHtmlAnchorElement)cut.Find("a")).Href.ShouldContain("https://ko-fi.com/Token");
    }
}
