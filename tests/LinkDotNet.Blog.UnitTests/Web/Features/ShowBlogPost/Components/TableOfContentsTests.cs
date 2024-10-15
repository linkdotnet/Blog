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
        links.Count.ShouldBe(3);
        var firstLink = links[0];
        firstLink.TextContent.ShouldBe("Header 1");
        firstLink.GetAttribute("href").ShouldBe("https://localhost#header-1");
        firstLink.ClassList.ShouldContain("ps-1");
        var secondLink = links[1];
        secondLink.TextContent.ShouldBe("Subheader 1.1");
        secondLink.GetAttribute("href").ShouldBe("https://localhost#subheader-1.1");
        secondLink.ClassList.ShouldContain("ps-2");
        var thirdLink = links[2];
        thirdLink.TextContent.ShouldBe("Header 2");
        thirdLink.GetAttribute("href").ShouldBe("https://localhost#header-2");
        thirdLink.ClassList.ShouldContain("ps-1");
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
        links.Count.ShouldBe(2);
        links[0].GetAttribute("href").ShouldBe("https://localhost#header-1");
        links[1].GetAttribute("href").ShouldBe("https://localhost#header-2");
    }
    
    [Theory]
    [InlineData("# This is `Header` 1", "This is Header 1")]
    [InlineData("# [This is a link](https://link.com)", "This is a link")]
    [InlineData("# **What** *if*", "What if")]
    [InlineData("# *[Link](link)*", "Link")]
    [InlineData("# Span&lt;T&gt; to the rescue", "Span<T> to the rescue")]
    public void ShouldCreateCorrectToc(string markdown, string expectedToc)
    {
        var cut = Render<TableOfContents>(p => p
            .Add(x => x.Content, markdown)
            .Add(x => x.CurrentUri, "https://localhost"));
        
        var link = cut.Find("nav a");
        link.TextContent.ShouldBe(expectedToc);
    }

    [Fact]
    public void EmptyTocShouldNotBeDisplayed()
    {
        var cut = Render<TableOfContents>(p => p
            .Add(x => x.Content, string.Empty)
            .Add(x => x.CurrentUri, "https://localhost"));
        
        cut.Markup.ShouldBeEmpty();
    }
}
