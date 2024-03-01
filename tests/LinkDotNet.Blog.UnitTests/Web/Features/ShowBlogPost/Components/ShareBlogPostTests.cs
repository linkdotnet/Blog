using System;
using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class ShareBlogPostTests : TestContext
{
    [Fact]
    public void ShouldCopyLinkToClipboard()
    {
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");
        var cut = RenderComponent<ShareBlogPost>();

        var element = cut.Find("#share-clipboard") as IHtmlAnchorElement;

        element.Should().NotBeNull();
        var onclick = element!.Attributes.FirstOrDefault(a => a.Name.Equals("onclick", StringComparison.InvariantCultureIgnoreCase));
        onclick.Should().NotBeNull();
        onclick!.Value.Should().Contain("blogPost/1");
    }

    [Fact]
    public void ShouldShareToLinkedIn()
    {
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");

        var cut = RenderComponent<ShareBlogPost>();

        var linkedInShare = (IHtmlAnchorElement)cut.Find("#share-linkedin");
        linkedInShare.Href.Should().Be("https://www.linkedin.com/shareArticle?mini=true&url=http://localhost/blogPost/1");
    }
}