using System.Linq;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class ShareBlogPostTests : TestContext
{
    [Fact]
    public void ShouldCopyLinkToClipboard()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped(_ => new Mock<IToastService>().Object);
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");
        var cut = RenderComponent<ShareBlogPost>();

        cut.Find("#share-clipboard").Click();

        var copyToClipboardInvocation = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "navigator.clipboard.writeText");
        copyToClipboardInvocation.Arguments[0].Should().Be("http://localhost/blogPost/1");
    }

    [Fact]
    public void ShouldShareToLinkedIn()
    {
        Services.AddScoped(_ => new Mock<IToastService>().Object);
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");

        var cut = RenderComponent<ShareBlogPost>();

        var linkedInShare = cut.Find("#share-linkedin").Attributes.Single(s => s.Name == "href").Value;
        linkedInShare.Should().Be("https://www.linkedin.com/shareArticle?mini=true&url=http://localhost/blogPost/1");
    }
}