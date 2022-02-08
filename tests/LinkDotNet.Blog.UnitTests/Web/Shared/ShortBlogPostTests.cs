using System.Linq;
using Bunit;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class ShortBlogPostTests : TestContext
{
    [Fact]
    public void ShouldOpenBlogPost()
    {
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "SomeId";
        var cut = RenderComponent<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var readMore = cut.Find(".read-more a");

        readMore.Attributes.Single(a => a.Name == "href").Value.Should().Be("/blogPost/SomeId");
    }

    [Fact]
    public void ShouldNavigateToEscapedTagSiteWhenClickingOnTag()
    {
        var blogPost = new BlogPostBuilder().WithTags("Tag 1").Build();
        var cut = RenderComponent<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var searchByTagLink = cut.Find(".goto-tag");

        searchByTagLink.Attributes.Single(a => a.Name == "href").Value.Should().Be("/searchByTag/Tag%201");
    }

    [Fact]
    public void ShouldCalculateReadTime()
    {
        var content = string.Join(' ', Enumerable.Repeat("word", 700)) + string.Join(' ', Enumerable.Repeat("<img>", 4));
        var blogPost = new BlogPostBuilder().WithContent(content).Build();
        var cut = RenderComponent<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var readTime = cut.Find(".read-time");

        readTime.TextContent.Should().Be("5 min");
    }
}