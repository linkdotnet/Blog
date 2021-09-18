using System;
using System.Linq;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public class BlogPostTests
    {
        [Fact]
        public void ShouldUpdateBlogPost()
        {
            var blogPostToUpdate = new BlogPostBuilder().Build();
            blogPostToUpdate.Id = "random-id";
            var blogPost = BlogPost.Create("Title", "Desc", "Content", "Url", true);
            blogPost.Id = "something else";

            blogPostToUpdate.Update(blogPost);

            blogPostToUpdate.Title.Should().Be("Title");
            blogPostToUpdate.ShortDescription.Should().Be("Desc");
            blogPostToUpdate.Content.Should().Be("Content");
            blogPostToUpdate.PreviewImageUrl.Should().Be("Url");
            blogPostToUpdate.IsPublished.Should().BeTrue();
            blogPostToUpdate.Tags.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ShouldTrimWhitespacesFromTags()
        {
            var blogPost = BlogPost.Create("Title", "Sub", "Content", "Preview", false, tags: new[] { " Tag 1", " Tag 2 ", });

            blogPost.Tags.Select(t => t.Content).Should().Contain("Tag 1");
            blogPost.Tags.Select(t => t.Content).Should().Contain("Tag 2");
        }

        [Fact]
        public void ShouldSetDateWhenGiven()
        {
            var somewhen = new DateTime(1991, 5, 17);

            var blog = BlogPost.Create("1", "2", "3", "4", false, somewhen);

            blog.UpdatedDate.Should().Be(somewhen);
        }
    }
}