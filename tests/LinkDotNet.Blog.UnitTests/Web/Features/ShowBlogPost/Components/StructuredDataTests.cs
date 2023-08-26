using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class StructuredDataTests : TestContext
{
    [Fact]
    public void ShouldRenderRawMarkup()
    {
        var cut = RenderComponent<StructuredData>(
            ps => ps.Add(p => p.Author, "Steven")
                .Add(p => p.Headline, "Headline")
                .Add(p => p.PreviewImage, "url1")
                .Add(p => p.PreviewFallbackImage, "url2"));

        var element = cut.Find("script");
        element.Should().NotBeNull();
    }
}
