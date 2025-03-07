using System;
using System.Linq;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class ShortBlogPostTests : BunitContext
{
    [Fact]
    public void ShouldOpenBlogPost()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "SomeId";
        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var readMore = cut.Find(".read-more a");

        readMore.Attributes.Single(a => a.Name == "href").Value.ShouldBe("/blogPost/SomeId/blogpost");
    }

    [Fact]
    public void ShouldNavigateToEscapedTagSiteWhenClickingOnTag()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().WithTags("Tag 1").Build();
        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        var searchByTagLink = cut.Find(".goto-tag");

        searchByTagLink.Attributes.Single(a => a.Name == "href").Value.ShouldBe("/searchByTag/Tag%201");
    }

    [Fact]
    public void WhenNoTagsAreGivenThenTagsAreNotShown()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.FindAll(".goto-tag").ShouldBeEmpty();
    }

    [Fact]
    public void GivenBlogPostThatIsScheduled_ThenIndicating()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(new DateTime(2099, 1, 1))
            .Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.Find(".schedule").ShouldNotBeNull();
    }

    [Fact]
    public void GivenBlogPostThatIsNotPublishedAndNotScheduled_ThenIndicating()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().IsPublished(false).Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.Find(".draft").ShouldNotBeNull();
    }

    [Fact]
    public void GivenBlogPostThatIsPublished_ThenNoDraft()
    {
        Services.AddScoped(_ => Substitute.For<IBookmarkService>());
        var blogPost = new BlogPostBuilder().IsPublished(true).Build();

        var cut = Render<ShortBlogPost>(
            p => p.Add(c => c.BlogPost, blogPost));

        cut.FindAll(".draft").ShouldBeEmpty();
        cut.FindAll(".scheduled").ShouldBeEmpty();
    }
}
