using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using Bunit;
using LinkDotNet.Blog.Web.Features.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class PreviewImageTests : TestContext
{
    [Fact]
    public void ShouldOfferImageWhenProvided()
    {
        var cut = RenderComponent<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.PreviewImageUrlFallback, "http://fallback.png/"));

        var picture = cut.Find("picture");

        var source = picture.Children[0] as IHtmlSourceElement;
        source.Should().NotBeNull();
        source.SourceSet.Should().Be("http://image.png/");
        var img = picture.Children[1] as IHtmlImageElement;
        img.Should().NotBeNull();
        img.Source.Should().Be("http://fallback.png/");
    }

    [Fact]
    public void ShouldOfferOnlyImageWhenNoFallbackProvided()
    {
        var cut = RenderComponent<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/"));

        var image = cut.Find("img").Unwrap() as IHtmlImageElement;

        image.Should().NotBeNull();
        image.Source.Should().Be("http://image.png/");
    }

    [Theory]
    [InlineData(true, "lazy")]
    [InlineData(false, "eager")]
    public void ShouldSetLazyLoadBehavior(bool lazyLoad, string expectedLazy)
    {
        var cut = RenderComponent<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.PreviewImageUrlFallback, "http://fallback.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var picture = cut.Find("picture");

        var img = picture.Children[1] as IHtmlImageElement;
        img.Should().NotBeNull();
        img.Attributes.FirstOrDefault(a => a.Name == "loading").Value.Should().Be(expectedLazy);
    }

    [Theory]
    [InlineData(true, "lazy")]
    [InlineData(false, "eager")]
    public void ShouldSetLazyLoadBehaviorNoFallback(bool lazyLoad, string expectedLazy)
    {
        var cut = RenderComponent<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var image = cut.Find("img").Unwrap() as IHtmlImageElement;

        image.Attributes.FirstOrDefault(a => a.Name == "loading").Value.Should().Be(expectedLazy);
    }
}