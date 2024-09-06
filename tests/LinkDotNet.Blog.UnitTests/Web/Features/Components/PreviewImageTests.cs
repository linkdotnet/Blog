using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Web.Features.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class PreviewImageTests : BunitContext
{
    [Fact]
    public void ShouldOfferImageWhenProvided()
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.PreviewImageUrlFallback, "http://fallback.png/"));

        var picture = cut.Find("picture");

        var source = picture.Children[0] as IHtmlSourceElement;
        source.ShouldNotBeNull();
        source.SourceSet.ShouldBe("http://image.png/");
        var img = picture.Children[1] as IHtmlImageElement;
        img.ShouldNotBeNull();
        img.Source.ShouldBe("http://fallback.png/");
    }

    [Fact]
    public void ShouldOfferOnlyImageWhenNoFallbackProvided()
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/"));

        var image = cut.Find("img") as IHtmlImageElement;

        image.ShouldNotBeNull();
        image.Source.ShouldBe("http://image.png/");
    }

    [Theory]
    [InlineData(true, "lazy")]
    [InlineData(false, "eager")]
    public void ShouldSetLazyLoadBehavior(bool lazyLoad, string expectedLazy)
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.PreviewImageUrlFallback, "http://fallback.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var picture = cut.Find("picture");

        var img = picture.Children[1] as IHtmlImageElement;
        img.ShouldNotBeNull();
        var loadingAttribute = img.Attributes.FirstOrDefault(a => a.Name == "loading");
        loadingAttribute.ShouldNotBeNull();
        loadingAttribute.Value.ShouldBe(expectedLazy);
    }

    [Theory]
    [InlineData(true, "lazy")]
    [InlineData(false, "eager")]
    public void ShouldSetLazyLoadBehaviorNoFallback(bool lazyLoad, string expectedLazy)
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var image = cut.Find("img") as IHtmlImageElement;

        image.ShouldNotBeNull();
        var loadingAttribute = image.Attributes.FirstOrDefault(a => a.Name == "loading");
        loadingAttribute.ShouldNotBeNull();
        loadingAttribute.Value.ShouldBe(expectedLazy);
    }

    [Theory]
    [InlineData("http://localhost/image.png", "image/png")]
    [InlineData("http://localhost/image.webp", "image/webp")]
    [InlineData("http://localhost/image.avif", "image/avif")]
    public void ShouldDetectFileTypes(string fileName, string mimeType)
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, fileName)
            .Add(p => p.PreviewImageUrlFallback, "not empty"));

        var picture = cut.Find("picture");

        var source = picture.Children[0] as IHtmlSourceElement;
        source.ShouldNotBeNull();
        source.Type.ShouldBe(mimeType);
    }

    [Theory]
    [InlineData(true, "async")]
    [InlineData(false, "auto")]
    public void ShouldSetDecodingBehavior(bool lazyLoad, string expectedBehaviour)
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.PreviewImageUrlFallback, "http://fallback.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var picture = cut.Find("picture");

        var img = picture.Children[1] as IHtmlImageElement;
        img.ShouldNotBeNull();
        var decodingAttribute = img.Attributes.FirstOrDefault(a => a.Name == "decoding");
        decodingAttribute.ShouldNotBeNull();
        decodingAttribute.Value.ShouldBe(expectedBehaviour);
    }

    [Theory]
    [InlineData(true, "async")]
    [InlineData(false, "auto")]
    public void ShouldSetDecodingBehaviorNoFallback(bool lazyLoad, string expectedBehaviour)
    {
        var cut = Render<PreviewImage>(ps => ps
            .Add(p => p.PreviewImageUrl, "http://image.png/")
            .Add(p => p.LazyLoadImage, lazyLoad));

        var image = cut.Find("img") as IHtmlImageElement;

        image.ShouldNotBeNull();
        var decodingAttribute = image.Attributes.FirstOrDefault(a => a.Name == "decoding");
        decodingAttribute.ShouldNotBeNull();
        decodingAttribute.Value.ShouldBe(expectedBehaviour);
    }
}