using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class OgDataTests : BunitContext
{
    [Fact]
    public void ShouldRenderMetaTagsWhenSet()
    {
        ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));

        var cut = Render<OgData>(p => p
            .Add(s => s.Title, "Title")
            .Add(s => s.Description, "Description")
            .Add(s => s.AbsolutePreviewImageUrl, "http://localhost/image.png")
            .Add(s => s.Keywords, "key1,key2"));

        AssertMetaTagExistsWithValue(cut, "title", "Title", "og:title");
        AssertMetaTagExistsWithValue(cut, "image", "http://localhost/image.png", "og:image");
        AssertMetaTagExistsWithValue(cut, "keywords", "key1,key2");
        AssertMetaTagExistsWithValue(cut, "description", "Description", "og:description");
    }

    [Fact]
    public void ShouldNotSetMetaInformationWhenNotProvided()
    {
        ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));

        var cut = Render<OgData>(p => p
            .Add(s => s.Title, "Title"));

        GetMetaTagExists(cut, "image").Should().BeFalse();
        GetMetaTagExists(cut, "keywords").Should().BeFalse();
        GetMetaTagExists(cut, "description").Should().BeFalse();
    }

    [Fact]
    public void ShouldSetCanoncialLink()
    {
        const string expectedUri = "https://steven.com/test";
        Services.GetRequiredService<NavigationManager>().NavigateTo(expectedUri);
        ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));

        var cut = Render<OgData>(p => p
            .Add(s => s.Title, "Title"));

        var link = cut.FindAll("link").FirstOrDefault(l => l.Attributes.Any(a => a.Name == "rel" && a.Value == "canonical")) as IHtmlLinkElement;

        link.Href.Should().Be(expectedUri);
    }

    [Fact]
    public void ShouldSetCanoncialLinkWithoutQueryParameter()
    {
        ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));
        Services.GetRequiredService<NavigationManager>().NavigateTo("https://localhost.com/site?query=2");

        var cut = Render<OgData>(p => p
            .Add(s => s.Title, "Title"));

        var link = cut.FindAll("link").FirstOrDefault(l => l.Attributes.Any(a => a.Name == "rel" && a.Value == "canonical")) as IHtmlLinkElement;
        link.Href.Should().Be("https://localhost.com/site");
    }

    private static void AssertMetaTagExistsWithValue(
        RenderedFragment cut,
        string metaTag,
        string metaTagValue,
        string ogPropertyName = null)
    {
        var metaTags = cut.FindAll("meta");
        var titleMeta = metaTags.SingleOrDefault(m => m.Attributes.Any(a => a.Value == metaTag));
        titleMeta.Should().NotBeNull();
        var titleMetaTag = (IHtmlMetaElement)titleMeta;
        titleMetaTag.Content.Should().Be(metaTagValue);
        if (ogPropertyName is not null)
        {
            titleMetaTag.Attributes.Any(a => a.Value == ogPropertyName).Should().BeTrue();
        }
    }

    private static bool GetMetaTagExists(
        RenderedFragment cut,
        string metaTag)
    {
        var metaTags = cut.FindAll("meta");
        return metaTags.Any(m => m.Attributes.Any(a => a.Value == metaTag));
    }
}