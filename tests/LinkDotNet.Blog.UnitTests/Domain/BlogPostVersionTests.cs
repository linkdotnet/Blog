using System;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class BlogPostVersionTests
{
    [Fact]
    public void CreateSnapshot_CopiesAllVersionableFields()
    {
        var updatedDate = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var blogPost = new BlogPostBuilder()
            .WithTitle("My Title")
            .WithShortDescription("My Short Description")
            .WithContent("My Content")
            .WithPreviewImageUrl("https://example.com/img.webp")
            .WithPreviewImageUrlFallback("https://example.com/img.jpg")
            .WithTags("tag1", "tag2")
            .WithAuthorName("John Doe")
            .WithUpdatedDate(updatedDate)
            .IsPublished(true)
            .Build();
        blogPost.Id = "post-123";

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, 3);

        snapshot.BlogPostId.ShouldBe("post-123");
        snapshot.VersionNumber.ShouldBe(3);
        snapshot.Title.ShouldBe("My Title");
        snapshot.ShortDescription.ShouldBe("My Short Description");
        snapshot.Content.ShouldBe("My Content");
        snapshot.PreviewImageUrl.ShouldBe("https://example.com/img.webp");
        snapshot.PreviewImageUrlFallback.ShouldBe("https://example.com/img.jpg");
        snapshot.Tags.ShouldBe(blogPost.Tags, ignoreOrder: false);
        snapshot.AuthorName.ShouldBe("John Doe");
        snapshot.UpdatedDate.ShouldBe(updatedDate);
        snapshot.IsPublished.ShouldBeTrue();
        snapshot.ReadingTimeInMinutes.ShouldBe(blogPost.ReadingTimeInMinutes);
    }

    [Fact]
    public void CreateSnapshot_SetsCreatedAtToApproximatelyNow()
    {
        var before = DateTime.UtcNow;
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "post-1";

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, 1);

        snapshot.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        snapshot.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void CreateSnapshot_DoesNotIncludeLikes()
    {
        // Likes are on BlogPost but must never appear on BlogPostVersion
        typeof(BlogPostVersion).GetProperty("Likes").ShouldBeNull();
    }

    [Fact]
    public void CreateSnapshot_DoesNotIncludeScheduledPublishDate()
    {
        // ScheduledPublishDate is editorial state, not part of content history
        typeof(BlogPostVersion).GetProperty("ScheduledPublishDate").ShouldBeNull();
    }

    [Fact]
    public void CreateSnapshot_ThrowsForNullBlogPost()
    {
        Action act = () => BlogPostVersion.CreateSnapshot(null!, 1);

        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CreateSnapshot_TagsAsStringJoinsWithComma()
    {
        var blogPost = new BlogPostBuilder().WithTags("csharp", "dotnet", "blazor").Build();
        blogPost.Id = "post-1";

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, 1);

        snapshot.TagsAsString.ShouldBe("csharp,dotnet,blazor");
    }

    [Fact]
    public void CreateSnapshot_WithNullAuthorName_PreservesNull()
    {
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = "post-1";

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, 1);

        snapshot.AuthorName.ShouldBeNull();
    }

    [Fact]
    public void CreateSnapshot_WithNullFallbackImageUrl_PreservesNull()
    {
        var blogPost = new BlogPostBuilder().WithPreviewImageUrlFallback(null).Build();
        blogPost.Id = "post-1";

        var snapshot = BlogPostVersion.CreateSnapshot(blogPost, 1);

        snapshot.PreviewImageUrlFallback.ShouldBeNull();
    }
}
