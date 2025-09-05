using System;
using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class BlogPostTests
{
    [Fact]
    public void ShouldUpdateBlogPost()
    {
        var blogPostToUpdate = new BlogPostBuilder().Build();
        blogPostToUpdate.Id = "random-id";
        var blogPost = BlogPost.Create("Title", "Desc", "Other Content", "Url", true, previewImageUrlFallback: "Url2", authorName: "Test Author");
        blogPost.Id = "something else";

        blogPostToUpdate.Update(blogPost);

        blogPostToUpdate.Title.ShouldBe("Title");
        blogPostToUpdate.ShortDescription.ShouldBe("Desc");
        blogPostToUpdate.Content.ShouldBe("Other Content");
        blogPostToUpdate.PreviewImageUrl.ShouldBe("Url");
        blogPostToUpdate.PreviewImageUrlFallback.ShouldBe("Url2");
        blogPostToUpdate.IsPublished.ShouldBeTrue();
        blogPostToUpdate.Tags.ShouldBeEmpty();
        blogPostToUpdate.Slug.ShouldNotBeNull();
        blogPostToUpdate.ReadingTimeInMinutes.ShouldBe(1);
        blogPostToUpdate.AuthorName.ShouldBe("Test Author");
    }

    [Fact]
    public void ShouldUpdateAuthorNameAsNullWhenNotGiven()
    {
        var blogPostToUpdate = new BlogPostBuilder().Build();
        blogPostToUpdate.Id = "random-id";
        var blogPost = BlogPost.Create("Title", "Desc", "Other Content", "Url", true, previewImageUrlFallback: "Url2");
        blogPost.Id = "something else";

        blogPostToUpdate.Update(blogPost);

        blogPostToUpdate.AuthorName.ShouldBeNull();
    }

    [Theory]
    [InlineData("blog title","blog-title")]
    [InlineData("blog      title", "blog-title")]
    [InlineData("blog +title", "blog-title")]
    [InlineData("blog/title", "blogtitle")]
    [InlineData("blog /title", "blog-title")]
    [InlineData("BLOG TITLE", "blog-title")]
    [InlineData("àccent", "accent")]
    [InlineData("get 100$ quick", "get-100-quick")]
    [InlineData("blog,title", "blogtitle")]
    [InlineData("blog?!title", "blogtitle")]
    [InlineData("blog----title", "blogtitle")]
    [InlineData("überaus gut", "uberaus-gut")]
    public void ShouldGenerateValidSlug(string title, string expectedSlug)
    {
        var blogPost = new BlogPostBuilder().WithTitle(title).Build();

        blogPost.Slug.ShouldBe(expectedSlug);
    }

    [Fact]
    public void ShouldUpdateTagsWhenExisting()
    {
        var blogPostToUpdate = new BlogPostBuilder().WithTags("tag 1").Build();
        blogPostToUpdate.Id = "random-id";
        var blogPost = new BlogPostBuilder().WithTags("tag 2").Build();
        blogPost.Id = "something else";

        blogPostToUpdate.Update(blogPost);

        blogPostToUpdate.Tags.ShouldHaveSingleItem();
        blogPostToUpdate.Tags.Single().ShouldBe("tag 2");
    }

    [Fact]
    public void ShouldTrimWhitespacesFromTags()
    {
        var blogPost = BlogPost.Create("Title", "Sub", "Content", "Preview", false, tags: new[] { " Tag 1", " Tag 2 ", });

        blogPost.Tags.ShouldContain("Tag 1");
        blogPost.Tags.ShouldContain("Tag 2");
    }

    [Fact]
    public void ShouldSetDateWhenGiven()
    {
        var somewhen = new DateTime(1991, 5, 17);

        var blog = BlogPost.Create("1", "2", "3", "4", false, somewhen);

        blog.UpdatedDate.ShouldBe(somewhen);
    }

    [Fact]
    public void ShouldNotDeleteTagsWhenSameReference()
    {
        var bp = new BlogPostBuilder().WithTags("tag 1").Build();

        bp.Update(bp);

        bp.Tags.ShouldHaveSingleItem();
        bp.Tags.Single().ShouldBe("tag 1");
    }

    [Fact]
    public void ShouldPublishBlogPost()
    {
        var date = new DateTime(2023, 3, 24);
        var bp = new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(date).Build();

        bp.Publish();

        bp.IsPublished.ShouldBeTrue();
        bp.ScheduledPublishDate.ShouldBeNull();
        bp.UpdatedDate.ShouldBe(date);
    }

    [Fact]
    public void ShouldThrowErrorWhenCreatingBlogPostThatIsPublishedAndHasScheduledPublishDate()
    {
        Action action = () => BlogPost.Create("1", "2", "3", "4", true, scheduledPublishDate: new DateTime(2023, 3, 24));

        action.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void ShouldUpdateScheduledPublishDate()
    {
        var blogPost = new BlogPostBuilder().Build();
        var bp = new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(new DateTime(2023, 3, 24)).Build();

        blogPost.Update(bp);

        blogPost.ScheduledPublishDate.ShouldBe(new DateTime(2023, 3, 24));
    }

    [Fact]
    public void GivenScheduledPublishDate_WhenCreating_ThenUpdateDateIsScheduledPublishDate()
    {
        var date = new DateTime(2023, 3, 24);

        var bp = BlogPost.Create("1", "2", "3", "4", false, scheduledPublishDate: date);

        bp.UpdatedDate.ShouldBe(date);
    }

    [Fact]
    public void GivenScheduledPublishDate_WhenCreating_ThenIsScheduledPublishDateIsTrue()
    {
        var date = new DateTime(2023, 3, 24);

        var bp = BlogPost.Create("1", "2", "3", "4", false, scheduledPublishDate: date);

        bp.IsScheduled.ShouldBeTrue();
    }

    [Fact]
    public void GivenBlogPostWithTags_WhenCreatingStringFromTags_ThenTagsAreSeparatedByComma()
    {
        var bp = new BlogPostBuilder().WithTags("tag 1", "tag 2").Build();

        var tags = bp.TagsAsString;

        tags.ShouldBe("tag 1,tag 2");
    }

    [Fact]
    public void GivenBlogPostWithNoTags_WhenCreatingStringFromTags_ThenEmptyString()
    {
        var bp = new BlogPostBuilder().Build();

        var tags = bp.TagsAsString;

        tags.ShouldBeEmpty();
    }
}
