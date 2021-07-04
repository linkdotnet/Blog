using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class ShortBlogPostTests
    {
        [Fact]
        public void ShouldOpenBlogPost()
        {
            using var ctx = new TestContext();
            var blogPost = new BlogPostBuilder().Build();
            blogPost.Id = "SomeId";
            var cut = ctx.RenderComponent<ShortBlogPost>(
                p => p.Add(c => c.BlogPost, blogPost));

            var readMore = cut.Find(".read-more a");

            readMore.Attributes.Single(a => a.Name == "href").Value.Should().Be("/SomeId");
        }

        [Fact]
        public void ShouldNavigateToEscapedTagSiteWhenClickingOnTag()
        {
            using var ctx = new TestContext();
            var blogPost = new BlogPostBuilder().WithTags("Tag 1").Build();
            var cut = ctx.RenderComponent<ShortBlogPost>(
                p => p.Add(c => c.BlogPost, blogPost));

            var searchByTagLink = cut.Find(".goto-tag");

            searchByTagLink.Attributes.Single(a => a.Name == "href").Value.Should().Be("/searchByTag/Tag%201");
        }
    }
}