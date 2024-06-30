using AngleSharp.Dom;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class TableOfContentsTests : BunitContext
{
    [Fact]
    public void ShouldShowTableOfContents()
    {
        const string content = """
                               # Header 1
                               ## Subheader 1.1
                               # Header 2
                               """;
        
        var cut = Render<TableOfContents>(p => p
            .Add(x => x.Content, content)
            .Add(x => x.CurrentUri, "https://localhost"));

        var links = cut.FindAll("nav a");
        links.Should().HaveCount(3);
        var firstLink = links[0];
        firstLink.TextContent.Should().Be("Header 1");
        firstLink.GetAttribute("href").Should().Be("https://localhost#header-1");
        firstLink.ClassList.Should().Contain("ps-1");
        var secondLink = links[1];
        secondLink.TextContent.Should().Be("Subheader 1.1");
        secondLink.GetAttribute("href").Should().Be("https://localhost#subheader-1.1");
        secondLink.ClassList.Should().Contain("ps-2");
        var thirdLink = links[2];
        thirdLink.TextContent.Should().Be("Header 2");
        thirdLink.GetAttribute("href").Should().Be("https://localhost#header-2");
        thirdLink.ClassList.Should().Contain("ps-1");
    }
    
    [Fact]
    public void ShouldSetAnchorLinkCorrectWhenAlreadyAnchorInUrl()
    {
        const string content = """
                               # Header 1
                               # Header 2
                               """;

        var cut = Render<TableOfContents>(p => p
            .Add(x => x.Content, content)
            .Add(x => x.CurrentUri, "https://localhost#header-1"));
        
        var links = cut.FindAll("nav a");
        links.Should().HaveCount(2);
        links[0].GetAttribute("href").Should().Be("https://localhost#header-1");
        links[1].GetAttribute("href").Should().Be("https://localhost#header-2");
    }
    
    [Fact]
    public void ShouldCreateCorrectTocWithCodeInHeadings()
    {
        const string content = "# This is `Header` 1";
        
        var cut = Render<TableOfContents>(p => p
            .Add(x => x.Content, content)
            .Add(x => x.CurrentUri, "https://localhost"));
        
        var link = cut.Find("nav a");
        link.TextContent.Should().Be("This is Header 1");
    }
}
