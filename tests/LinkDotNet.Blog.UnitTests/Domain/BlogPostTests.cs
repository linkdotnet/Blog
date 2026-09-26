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

    [Fact]
    public void ShouldIncreaseLikesWhenLiked()
    {
        var blogPost = new BlogPostBuilder().WithLikes(1).Build();

        blogPost.Like();

        blogPost.Likes.ShouldBe(2);
    }

    [Fact]
    public void ShouldNotDropLikesBelowZeroWhenUnliked()
    {
        var blogPost = new BlogPostBuilder().WithLikes(0).Build();

        blogPost.Unlike();

        blogPost.Likes.ShouldBe(0);
    }

    [Fact]
    public void ShouldSnapshotCurrentStateWhenRevised()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Old").WithContent("Old content").WithLikes(3).Build();
        blogPost.Id = "post-1";
        var changes = new BlogPostBuilder().WithTitle("New").WithContent("New content").Build();

        var snapshot = blogPost.Revise(changes, 4);

        snapshot.BlogPostId.ShouldBe("post-1");
        snapshot.VersionNumber.ShouldBe(5);
        snapshot.Title.ShouldBe("Old");
        snapshot.Content.ShouldBe("Old content");
        blogPost.Title.ShouldBe("New");
        blogPost.Content.ShouldBe("New content");
        blogPost.Id.ShouldBe("post-1");
        blogPost.Likes.ShouldBe(3);
    }

    [Fact]
    public void ShouldRestoreVersionAndSnapshotCurrentState()
    {
        var versionDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var blogPost = new BlogPostBuilder().WithTitle("Old").WithContent("Old").WithUpdatedDate(versionDate).Build();
        blogPost.Id = "post-1";
        var version = BlogPostVersion.CreateSnapshot(blogPost, 1);
        blogPost.Update(new BlogPostBuilder().WithTitle("Current").WithContent(string.Join(' ', Enumerable.Repeat("word", 1000))).Build());

        var snapshot = blogPost.RestoreFrom(version, 1);

        snapshot.VersionNumber.ShouldBe(2);
        snapshot.Title.ShouldBe("Current");
        blogPost.Title.ShouldBe("Old");
        blogPost.UpdatedDate.ShouldBe(versionDate);
        blogPost.ReadingTimeInMinutes.ShouldBe(version.ReadingTimeInMinutes);
    }

    [Fact]
    public void ShouldClearScheduleWhenRestoringPublishedVersion()
    {
        var blogPost = new BlogPostBuilder().IsPublished(true).Build();
        blogPost.Id = "post-1";
        var version = BlogPostVersion.CreateSnapshot(blogPost, 1);
        blogPost.Update(new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(DateTime.UtcNow.AddDays(1)).Build());

        blogPost.RestoreFrom(version, 1);

        blogPost.IsPublished.ShouldBeTrue();
        blogPost.ScheduledPublishDate.ShouldBeNull();
    }

    [Fact]
    public void ShouldKeepScheduleWhenRestoringUnpublishedVersion()
    {
        var scheduledDate = DateTime.UtcNow.AddDays(1);
        var blogPost = new BlogPostBuilder().IsPublished(false).Build();
        blogPost.Id = "post-1";
        var version = BlogPostVersion.CreateSnapshot(blogPost, 1);
        blogPost.Update(new BlogPostBuilder().IsPublished(false).WithScheduledPublishDate(scheduledDate).Build());

        blogPost.RestoreFrom(version, 1);

        blogPost.ScheduledPublishDate.ShouldBe(scheduledDate);
    }

    [Fact]
    public void ShouldNotRestoreVersionOfAnotherBlogPost()
    {
        var other = new BlogPostBuilder().Build();
        other.Id = "other";
        var version = BlogPostVersion.CreateSnapshot(other, 1);
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "post-1";

        Should.Throw<InvalidOperationException>(() => blogPost.RestoreFrom(version, 1));
    }

    [Fact]
    public void ShouldCreateSameSlugAsBlogPost()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Hello World: C# ĂŚ Tips").Build();

        BlogPost.CreateSlug(blogPost.Title).ShouldBe(blogPost.Slug);
    }
}
