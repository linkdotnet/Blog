using System.Linq;
using AngleSharp.Html.Dom;
using Bunit;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.AspNetCore.Components.Web;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class OgDataTests : TestContext
{
    [Fact]
    public void ShouldRenderMetaTagsWhenSet()
    {
        ComponentFactories.AddStub<HeadContent>(ps => ps.Get(p => p.ChildContent));

        var cut = RenderComponent<OgData>(p => p
            .Add(s => s.Title, "Title")
            .Add(s => s.Description, "Description")
            .Add(s => s.AbsolutePreviewImageUrl, "http://localhost/image.png")
            .Add(s => s.Keywords, "key1,key2"));

        AssertMetaTagExistsWithValue(cut, "title", "Title", "og:title");
        AssertMetaTagExistsWithValue(cut, "image", "http://localhost/image.png", "og:image");
        AssertMetaTagExistsWithValue(cut, "keywords", "key1,key2");
        AssertMetaTagExistsWithValue(cut, "description", "Description", "og:description");
    }

    private static void AssertMetaTagExistsWithValue(
        IRenderedFragment cut,
        string metaTag,
        string metaTagValue,
        string ogPropertyName = null)
    {
        var metaTags = cut.FindAll("meta");
        var titleMeta = metaTags.SingleOrDefault(m => m.Attributes.Any(a => a.Value == metaTag));
        titleMeta.Should().NotBeNull();
        var titleMetaTag = (IHtmlMetaElement)titleMeta;
        titleMetaTag.Content.Should().Be(metaTagValue);
        if (ogPropertyName != null)
        {
            titleMetaTag.Attributes.Any(a => a.Value == ogPropertyName).Should().BeTrue();
        }
    }
}