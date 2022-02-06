using System;
using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class ShareBlogPostTests : TestContext
{
    [Fact]
    public void ShouldCopyLinkToClipboard()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped(_ => Mock.Of<IToastService>());
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");
        var cut = RenderComponent<ShareBlogPost>();

        cut.Find("#share-clipboard").Click();

        var copyToClipboardInvocation = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "navigator.clipboard.writeText");
        copyToClipboardInvocation.Arguments[0].Should().Be("http://localhost/blogPost/1");
    }

    [Fact]
    public void ShouldShareToLinkedIn()
    {
        Services.AddScoped(_ => Mock.Of<IToastService>());
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("blogPost/1");

        var cut = RenderComponent<ShareBlogPost>();

        var linkedInShare = (IHtmlAnchorElement)cut.Find("#share-linkedin").Unwrap();
        linkedInShare.Href.Should().Be("https://www.linkedin.com/shareArticle?mini=true&url=http://localhost/blogPost/1");
    }

    [Fact]
    public void ShouldNotCrashWhenCopyingLinkNotWorking()
    {
        Services.AddScoped(_ => Mock.Of<IToastService>());
        JSInterop.SetupVoid(s => s.InvocationMethodName == "navigator.clipboard.writeText").SetException(new Exception());
        var cut = RenderComponent<ShareBlogPost>();

        var act = () => cut.Find("#share-clipboard").Click();

        act.Should().NotThrow<Exception>();
    }
}