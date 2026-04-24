using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using Microsoft.EntityFrameworkCore;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BlogPostEditor;

public class BlogPostVersionServiceTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly IBlogPostVersionService sut;

    public BlogPostVersionServiceTests()
    {
        sut = new BlogPostVersionService(DbContextFactory, Repository);
    }

    [Fact]
    public async Task SaveNewVersionAsync_CreatesSnapshotOfCurrentStateBeforeUpdate()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Original Title").WithShortDescription("Original Desc").Build();
        await Repository.StoreAsync(blogPost);

        var updated = new BlogPostBuilder().WithTitle("Updated Title").WithShortDescription("Updated Desc").Build();
        updated.Id = blogPost.Id;

        await sut.SaveNewVersionAsync(blogPost, updated);

        var versions = await DbContext.BlogPostVersions
            .Where(v => v.BlogPostId == blogPost.Id)
            .ToListAsync(TestContext.Current.CancellationToken);

        versions.ShouldHaveSingleItem();
        versions[0].Title.ShouldBe("Original Title");
        versions[0].ShortDescription.ShouldBe("Original Desc");
        versions[0].VersionNumber.ShouldBe(1);
    }

    [Fact]
    public async Task SaveNewVersionAsync_UpdatesBlogPostInDatabase()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Original").WithShortDescription("Old Desc").Build();
        await Repository.StoreAsync(blogPost);

        var updated = new BlogPostBuilder().WithTitle("Updated").WithShortDescription("New Desc").Build();
        updated.Id = blogPost.Id;

        await sut.SaveNewVersionAsync(blogPost, updated);

        var fromDb = await DbContext.BlogPosts
            .AsNoTracking()
            .SingleAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken);
        fromDb.Title.ShouldBe("Updated");
        fromDb.ShortDescription.ShouldBe("New Desc");
    }

    [Fact]
    public async Task SaveNewVersionAsync_VersionNumbersAreSequential()
    {
        var blogPost = new BlogPostBuilder().WithTitle("V1").Build();
        await Repository.StoreAsync(blogPost);

        var v2 = new BlogPostBuilder().WithTitle("V2").Build();
        v2.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, v2);

        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var v3 = new BlogPostBuilder().WithTitle("V3").Build();
        v3.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(refreshed!, v3);

        var versions = await DbContext.BlogPostVersions
            .Where(v => v.BlogPostId == blogPost.Id)
            .OrderBy(v => v.VersionNumber)
            .ToListAsync(TestContext.Current.CancellationToken);

        versions.Count.ShouldBe(2);
        versions[0].VersionNumber.ShouldBe(1);
        versions[0].Title.ShouldBe("V1");
        versions[1].VersionNumber.ShouldBe(2);
        versions[1].Title.ShouldBe("V2");
    }

    [Fact]
    public async Task SaveNewVersionAsync_DoesNotChangeIdOrLikes()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Original").Build();
        blogPost.Likes = 42;
        await Repository.StoreAsync(blogPost);
        var originalId = blogPost.Id;

        var updated = new BlogPostBuilder().WithTitle("Updated").Build();
        updated.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, updated);

        var fromDb = await DbContext.BlogPosts
            .AsNoTracking()
            .SingleAsync(b => b.Id == originalId, TestContext.Current.CancellationToken);
        fromDb.Id.ShouldBe(originalId);
        fromDb.Likes.ShouldBe(42);
    }

    [Fact]
    public async Task GetVersionHistoryAsync_ReturnsVersionsOrderedByVersionNumberDescending()
    {
        var blogPost = new BlogPostBuilder().WithTitle("V1").Build();
        await Repository.StoreAsync(blogPost);

        var v2 = new BlogPostBuilder().WithTitle("V2").Build();
        v2.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, v2);

        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var v3 = new BlogPostBuilder().WithTitle("V3").Build();
        v3.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(refreshed!, v3);

        var history = await sut.GetVersionHistoryAsync(blogPost.Id);

        history.Count.ShouldBe(2);
        history[0].VersionNumber.ShouldBe(2);
        history[1].VersionNumber.ShouldBe(1);
    }

    [Fact]
    public async Task GetVersionHistoryAsync_ReturnsEmptyForPostWithNoVersions()
    {
        var blogPost = new BlogPostBuilder().Build();
        await Repository.StoreAsync(blogPost);

        var history = await sut.GetVersionHistoryAsync(blogPost.Id);

        history.ShouldBeEmpty();
    }

    [Fact]
    public async Task RestoreVersionAsync_CreatesPreRestoreSnapshotBeforeOverwriting()
    {
        var blogPost = new BlogPostBuilder().WithTitle("V1").Build();
        await Repository.StoreAsync(blogPost);
        var v2 = new BlogPostBuilder().WithTitle("V2").Build();
        v2.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, v2);

        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var history = await sut.GetVersionHistoryAsync(blogPost.Id);
        var v1Snapshot = history.Single(v => v.VersionNumber == 1);

        await sut.RestoreVersionAsync(refreshed!, v1Snapshot);

        var allVersions = await DbContext.BlogPostVersions
            .Where(v => v.BlogPostId == blogPost.Id)
            .OrderBy(v => v.VersionNumber)
            .ToListAsync(TestContext.Current.CancellationToken);

        allVersions.Count.ShouldBe(2);
        allVersions[1].Title.ShouldBe("V2"); // v2 = pre-restore snapshot of current "V2" state
    }

    [Fact]
    public async Task RestoreVersionAsync_AppliesTargetVersionFieldsToBlogPost()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Original").WithShortDescription("Original Desc").Build();
        await Repository.StoreAsync(blogPost);
        var updated = new BlogPostBuilder().WithTitle("Updated").WithShortDescription("Updated Desc").Build();
        updated.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, updated);

        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var history = await sut.GetVersionHistoryAsync(blogPost.Id);
        var v1 = history.Single(v => v.VersionNumber == 1);

        await sut.RestoreVersionAsync(refreshed!, v1);

        var fromDb = await DbContext.BlogPosts
            .AsNoTracking()
            .SingleAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken);
        fromDb.Title.ShouldBe("Original");
        fromDb.ShortDescription.ShouldBe("Original Desc");
    }

    [Fact]
    public async Task RestoreVersionAsync_DoesNotRestoreScheduledPublishDate()
    {
        var scheduledDate = DateTime.UtcNow.AddDays(7);
        var blogPost = new BlogPostBuilder()
            .WithTitle("Scheduled Post")
            .WithScheduledPublishDate(scheduledDate)
            .IsPublished(false)
            .Build();
        await Repository.StoreAsync(blogPost);

        // Save a version (snapshot of the scheduled post)
        var published = new BlogPostBuilder().WithTitle("Now Published").IsPublished(true).Build();
        published.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, published);

        // Restore v1 which had a scheduled date — the date must NOT come back
        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var history = await sut.GetVersionHistoryAsync(blogPost.Id);
        var v1 = history.Single(v => v.VersionNumber == 1);

        await sut.RestoreVersionAsync(refreshed!, v1);

        var fromDb = await DbContext.BlogPosts
            .AsNoTracking()
            .SingleAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken);
        fromDb.ScheduledPublishDate.ShouldBeNull();
    }

    [Fact]
    public async Task RestoreVersionAsync_UpdatedDateIsRestoredFromVersion()
    {
        var originalDate = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc);
        var blogPost = new BlogPostBuilder()
            .WithTitle("Post")
            .WithUpdatedDate(originalDate)
            .Build();
        await Repository.StoreAsync(blogPost);

        var updated = new BlogPostBuilder()
            .WithTitle("Post Updated")
            .WithUpdatedDate(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();
        updated.Id = blogPost.Id;
        await sut.SaveNewVersionAsync(blogPost, updated);

        var refreshed = await Repository.GetByIdAsync(blogPost.Id);
        var history = await sut.GetVersionHistoryAsync(blogPost.Id);
        var v1 = history.Single(v => v.VersionNumber == 1);

        await sut.RestoreVersionAsync(refreshed!, v1);

        var fromDb = await DbContext.BlogPosts
            .AsNoTracking()
            .SingleAsync(b => b.Id == blogPost.Id, TestContext.Current.CancellationToken);
        fromDb.UpdatedDate.ShouldBe(originalDate);
    }

    [Fact]
    public async Task SaveNewVersionAsync_ThrowsForNullCurrentBlogPost()
    {
        Func<Task> act = () => sut.SaveNewVersionAsync(null!, new BlogPostBuilder().Build()).AsTask();

        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SaveNewVersionAsync_ThrowsForNullUpdatedBlogPost()
    {
        var blogPost = new BlogPostBuilder().Build();
        await Repository.StoreAsync(blogPost);

        Func<Task> act = () => sut.SaveNewVersionAsync(blogPost, null!).AsTask();

        await act.ShouldThrowAsync<ArgumentNullException>();
    }
}
