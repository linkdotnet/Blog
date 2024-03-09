using System;
using System.Linq;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class ShortBlogPostTests : BunitContext
{
    [Fact]
    public void ShouldOpenBlogPost()
    {
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "SomeId";
        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var readMore = cut.Find(".read-more a");

        readMore.Attributes.Single(a => a.Name == "href").Value.Should().Be("/blogPost/SomeId/blogpost");
    }

    [Fact]
    public void ShouldNavigateToEscapedTagSiteWhenClickingOnTag()
    {
        var blogPost = new BlogPostBuilder().WithTags("Tag 1").Build();
        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var searchByTagLink = cut.Find(".goto-tag");

        searchByTagLink.Attributes.Single(a => a.Name == "href").Value.Should().Be("/searchByTag/Tag%201");
    }

    [Fact]
    public void WhenNoTagsAreGivenThenTagsAreNotShown()
    {
        var blogPost = new BlogPostBuilder().Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.FindAll(".goto-tag").Should().BeEmpty();
    }

    [Fact]
    public void GivenBlogPostThatIsScheduled_ThenIndicating()
    {
        var blogPost = new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(new DateTime(2099, 1, 1))
            .Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.Find(".schedule").Should().NotBeNull();
    }

    [Fact]
    public void GivenBlogPostThatIsNotPublishedAndNotScheduled_ThenIndicating()
    {
        var blogPost = new BlogPostBuilder().IsPublished(false).Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.Find(".draft").Should().NotBeNull();
    }

    [Fact]
    public void GivenBlogPostThatIsPublished_ThenNoDraft()
    {
        var blogPost = new BlogPostBuilder().IsPublished(true).Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.FindAll(".draft").Should().BeEmpty();
        cut.FindAll(".scheduled").Should().BeEmpty();
    }
}
