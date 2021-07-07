using System.Linq;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Admin;
using LinkDotNet.Domain;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Admin
{
    public class CreateNewBlogPostTests
    {
        [Fact]
        public void ShouldCreateNewBlogPostWhenValidDataGiven()
        {
            using var ctx = new TestContext();
            BlogPost blogPost = null;
            var cut = ctx.RenderComponent<CreateNewBlogPost>(
                p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
            cut.Find("#title").Change("My Title");
            cut.Find("#short").Change("My short Description");
            cut.Find("#content").Change("My content");
            cut.Find("#preview").Change("My preview url");
            cut.Find("#published").Change(false);
            cut.Find("#tags").Change("Tag1,Tag2,Tag3");

            cut.Find("form").Submit();

            cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
            blogPost.Should().NotBeNull();
            blogPost.Title.Should().Be("My Title");
            blogPost.ShortDescription.Should().Be("My short Description");
            blogPost.Content.Should().Be("My content");
            blogPost.PreviewImageUrl.Should().Be("My preview url");
            blogPost.IsPublished.Should().BeFalse();
            blogPost.Tags.Should().HaveCount(3);
            blogPost.Tags.Select(t => t.Content).Should().Contain(new[] { "Tag1", "Tag2", "Tag3" });
        }

        [Fact]
        public void ShouldFillGivenBlogPost()
        {
            using var ctx = new TestContext();
            var blogPost = new BlogPostBuilder().WithTitle("Title").WithShortDescription("Desc").WithContent("Content").Build();
            BlogPost blogPostFromComponent = null;
            var cut = ctx.RenderComponent<CreateNewBlogPost>(
                p =>
                    p.Add(c => c.OnBlogPostCreated, bp => blogPostFromComponent = bp)
                     .Add(c => c.BlogPost, blogPost));
            cut.Find("#title").Change("My new Title");

            cut.Find("form").Submit();

            cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
            blogPostFromComponent.Should().NotBeNull();
            blogPostFromComponent.Title.Should().Be("My new Title");
            blogPostFromComponent.ShortDescription.Should().Be("Desc");
            blogPostFromComponent.Content.Should().Be("Content");
        }

        [Fact]
        public void ShouldNotDeleteModelWhenSet()
        {
            using var ctx = new TestContext();
            BlogPost blogPost = null;
            var cut = ctx.RenderComponent<CreateNewBlogPost>(
                p => p.Add(c => c.ClearAfterCreated, true)
                    .Add(c => c.OnBlogPostCreated, post => blogPost = post));
            cut.Find("#title").Change("My Title");
            cut.Find("#short").Change("My short Description");
            cut.Find("#content").Change("My content");
            cut.Find("#preview").Change("My preview url");
            cut.Find("#tags").Change("Tag1,Tag2,Tag3");
            cut.Find("form").Submit();
            blogPost = null;

            cut.Find("form").Submit();

            blogPost.Should().BeNull();
        }

        [Fact]
        public void ShouldNotDeleteModelWhenNotSet()
        {
            using var ctx = new TestContext();
            BlogPost blogPost = null;
            var cut = ctx.RenderComponent<CreateNewBlogPost>(
                p => p.Add(c => c.ClearAfterCreated, false)
                    .Add(c => c.OnBlogPostCreated, post => blogPost = post));
            cut.Find("#title").Change("My Title");
            cut.Find("#short").Change("My short Description");
            cut.Find("#content").Change("My content");
            cut.Find("#preview").Change("My preview url");
            cut.Find("#tags").Change("Tag1,Tag2,Tag3");
            cut.Find("form").Submit();
            blogPost = null;

            cut.Find("form").Submit();

            blogPost.Should().NotBeNull();
        }
    }
}