using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
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
    }
}