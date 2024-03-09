using System;
using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class ShareBlogPostTests : BunitContext
{
    [Fact]
    public void ShouldCopyLinkToClipboard()
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("blogPost/1");
        var cut = Render<ShareBlogPost>();

        var element = cut.Find("#share-clipboard") as IHtmlAnchorElement;

        element.Should().NotBeNull();
        var onclick = element!.Attributes.FirstOrDefault(a => a.Name.Equals("onclick", StringComparison.InvariantCultureIgnoreCase));
        onclick.Should().NotBeNull();
        onclick!.Value.Should().Contain("blogPost/1");
    }

    [Fact]
    public void ShouldShareToLinkedIn()
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("blogPost/1");

        var cut = Render<ShareBlogPost>();

        var linkedInShare = (IHtmlAnchorElement)cut.Find("#share-linkedin");
        linkedInShare.Href.Should().Be("https://www.linkedin.com/shareArticle?mini=true&url=http://localhost/blogPost/1");
    }
}