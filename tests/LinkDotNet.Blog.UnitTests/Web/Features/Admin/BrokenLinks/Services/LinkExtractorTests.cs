using System;
using System.Linq;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BrokenLinks.Services;

public class LinkExtractorTests
{
    [Fact]
    public void ShouldExtractLinksImagesAndAutoLinks()
    {
        const string markdown = """
                                [Link](https://link.com/page)
                                ![Image](https://images.com/image.png)
                                <https://autolink.com>
                                Bare url https://bare.com/path
                                """;

        var urls = LinkExtractor.ExtractAbsoluteUrls(markdown).Select(u => u.ToString());

        urls.ShouldBe(
            ["https://link.com/page", "https://images.com/image.png", "https://bare.com/path", "https://autolink.com/"],
            ignoreOrder: true);
    }

    [Fact]
    public void ShouldIgnoreRelativeAnchorMailAndNonHttpLinks()
    {
        const string markdown = """
                                [Relative](/blogPost/1)
                                [Anchor](#heading)
                                [Mail](mailto:me@me.com)
                                <me@me.com>
                                [Ftp](ftp://files.com)
                                [Js](javascript:alert(1))
                                """;

        LinkExtractor.ExtractAbsoluteUrls(markdown).ShouldBeEmpty();
    }

    [Fact]
    public void ShouldReturnDistinctUrls()
    {
        const string markdown = "[One](https://link.com) and [Two](https://link.com)";

        LinkExtractor.ExtractAbsoluteUrls(markdown).ShouldHaveSingleItem().ShouldBe(new Uri("https://link.com"));
    }

    [Fact]
    public void ShouldIgnoreLinksInCodeBlocks()
    {
        const string markdown = """
                                ```
                                [Link](https://link.com)
                                ```
                                `https://inline.com`
                                """;

        LinkExtractor.ExtractAbsoluteUrls(markdown).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldReturnEmptyForEmptyContent(string markdown)
    {
        LinkExtractor.ExtractAbsoluteUrls(markdown).ShouldBeEmpty();
    }
}
