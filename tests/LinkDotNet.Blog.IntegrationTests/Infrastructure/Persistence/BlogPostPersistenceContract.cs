using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence;

public abstract class BlogPostPersistenceContract : IAsyncLifetime
{
    private IBlogPostPersistenceHarness harness = default!;

    private IBlogPostRepository Sut => harness.Repository;

    public async ValueTask InitializeAsync() => harness = await CreateHarnessAsync();

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await harness.DisposeAsync();
    }

    [Fact]
    public async Task ShouldReturnNullForUnknownBlogPost()
    {
        var blogPost = await Sut.GetAsync("unknown");

        blogPost.ShouldBeNull();
    }

    [Fact]
    public async Task ShouldNumberVersionsSequentiallyAndReturnHistoryImmediately()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("V1").Build());

        await ReviseAsync(blogPost.Id, "V2");
        var historyAfterFirstRevision = await Sut.GetVersionHistoryAsync(blogPost.Id);
        await ReviseAsync(blogPost.Id, "V3");
        var history = await Sut.GetVersionHistoryAsync(blogPost.Id);

        historyAfterFirstRevision.ShouldHaveSingleItem().VersionNumber.ShouldBe(1);
        history.Select(v => v.VersionNumber).ShouldBe([2, 1]);
        history.Select(v => v.Title).ShouldBe(["V2", "V1"]);
        (await harness.LoadBlogPostAsync(blogPost.Id))!.Title.ShouldBe("V3");
    }

    [Fact]
    public async Task ShouldKeepIdAndLikesWhenRevised()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Old").WithLikes(3).Build());

        await ReviseAsync(blogPost.Id, "New");

        var fromDb = await harness.LoadBlogPostAsync(blogPost.Id);
        fromDb.ShouldNotBeNull();
        fromDb.Id.ShouldBe(blogPost.Id);
        fromDb.Title.ShouldBe("New");
        fromDb.Likes.ShouldBe(3);
    }

    [Fact]
    public async Task ShouldSnapshotCurrentStateBeforeRestoring()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Original").Build());
        await ReviseAsync(blogPost.Id, "Changed");
        var original = (await Sut.GetVersionHistoryAsync(blogPost.Id)).Single();

        await Sut.ReviseAsync(blogPost.Id, (post, latest) => post.RestoreFrom(original, latest));

        var history = await Sut.GetVersionHistoryAsync(blogPost.Id);
        history.Count.ShouldBe(2);
        history[0].VersionNumber.ShouldBe(2);
        history[0].Title.ShouldBe("Changed");
        (await harness.LoadBlogPostAsync(blogPost.Id))!.Title.ShouldBe("Original");
    }

    [Fact]
    public async Task ShouldKeepWorkingWithVersionsStoredBeforeDeterministicIds()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Legacy").Build());
        var legacyVersion = BlogPostVersion.CreateSnapshot(blogPost, 1);
        legacyVersion.Id = null!;
        await harness.StoreAsync(legacyVersion);

        await ReviseAsync(blogPost.Id, "Changed");
        var legacyFromHistory = (await Sut.GetVersionHistoryAsync(blogPost.Id)).Single(v => v.VersionNumber == 1);
        await Sut.ReviseAsync(blogPost.Id, (post, latest) => post.RestoreFrom(legacyFromHistory, latest));

        var history = await Sut.GetVersionHistoryAsync(blogPost.Id);
        history.Select(v => v.VersionNumber).ShouldBe([3, 2, 1]);
        history.Single(v => v.VersionNumber == 1).Id.ShouldNotBe(BlogPostVersion.CreateId(blogPost.Id, 1));
        (await harness.LoadBlogPostAsync(blogPost.Id))!.Title.ShouldBe("Legacy");

        await Sut.DeleteAsync(blogPost.Id);
        (await harness.CountVersionsAsync(blogPost.Id)).ShouldBe(0);
    }

    [Fact]
    public async Task ShouldThrowConflictAndLeaveBlogPostUnchangedWhenVersionNumberIsTaken()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Original").Build());
        var changes = new BlogPostBuilder().WithTitle("Changed").Build();

        await Should.ThrowAsync<BlogPostRevisionConflictException>(async () =>
            await Sut.ReviseAsync(blogPost.Id, (post, latest) =>
            {
                harness.InsertVersion(BlogPostVersion.CreateSnapshot(post, latest + 1));
                return post.Revise(changes, latest);
            }));

        (await harness.LoadBlogPostAsync(blogPost.Id))!.Title.ShouldBe("Original");
        (await harness.CountVersionsAsync(blogPost.Id)).ShouldBe(1);
    }

    [Fact]
    public async Task ShouldNotLoseLikeThatHappensDuringRevision()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Old").WithLikes(1).Build());
        var changes = new BlogPostBuilder().WithTitle("New").Build();
        var hasLiked = false;

        BlogPostVersion Revise(BlogPost post, int latest)
        {
            if (!hasLiked)
            {
                hasLiked = true;
                harness.Like(post.Id);
            }

            return post.Revise(changes, latest);
        }

        try
        {
            await Sut.ReviseAsync(blogPost.Id, Revise);
        }
        catch (BlogPostRevisionConflictException)
        {
            await Sut.ReviseAsync(blogPost.Id, Revise);
        }

        var fromDb = await harness.LoadBlogPostAsync(blogPost.Id);
        fromDb!.Likes.ShouldBe(2);
        fromDb.Title.ShouldBe("New");
    }

    [Fact]
    public async Task ShouldLikeAndUnlikeWithoutGoingBelowZero()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithLikes(0).Build());

        await Sut.LikeAsync(blogPost.Id);
        await Sut.LikeAsync(blogPost.Id);
        var likedTwice = (await harness.LoadBlogPostAsync(blogPost.Id))!.Likes;
        await Sut.UnlikeAsync(blogPost.Id);
        await Sut.UnlikeAsync(blogPost.Id);
        await Sut.UnlikeAsync(blogPost.Id);

        likedTwice.ShouldBe(2);
        (await harness.LoadBlogPostAsync(blogPost.Id))!.Likes.ShouldBe(0);
    }

    [Fact]
    public async Task ShouldDeleteBlogPostWithItsVersions()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().Build());
        var otherBlogPost = await StoreAsync(new BlogPostBuilder().Build());
        await ReviseAsync(blogPost.Id, "Changed");
        await ReviseAsync(otherBlogPost.Id, "Changed");

        await Sut.DeleteAsync(blogPost.Id);

        (await harness.LoadBlogPostAsync(blogPost.Id)).ShouldBeNull();
        (await harness.CountVersionsAsync(blogPost.Id)).ShouldBe(0);
        (await harness.CountVersionsAsync(otherBlogPost.Id)).ShouldBe(1);
    }

    [Fact]
    public async Task ShouldReturnNoPageForUnknownBlogPost()
    {
        var page = await harness.PageQuery.GetAsync("unknown");

        page.ShouldBeNull();
    }

    [Fact]
    public async Task ShouldReturnPageWithAllShortCodes()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().WithTitle("Title").Build());
        await harness.StoreAsync(ShortCode.Create("one", "1"));
        await harness.StoreAsync(ShortCode.Create("two", "2"));

        var page = await harness.PageQuery.GetAsync(blogPost.Id);

        page.ShouldNotBeNull();
        page.BlogPost.Title.ShouldBe("Title");
        page.ShortCodes.Select(s => s.Name).ShouldBe(["one", "two"], ignoreOrder: true);
        page.ShortCodes.Single(s => s.Name == "two").MarkdownContent.ShouldBe("2");
        page.SimilarBlogPosts.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnOnlyPublishedSimilarBlogPostsInSimilarityOrder()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().Build());
        var mostSimilar = await StoreAsync(new BlogPostBuilder().WithTitle("Most Similar").WithTags("tag").IsPublished().Build());
        var unpublished = await StoreAsync(new BlogPostBuilder().WithTitle("Unpublished").IsPublished(false).Build());
        var leastSimilar = await StoreAsync(new BlogPostBuilder().WithTitle("Least Similar").IsPublished().Build());
        await harness.StoreAsync(new SimilarBlogPost
        {
            Id = SimilarBlogPost.IdFor(blogPost.Id),
            SimilarBlogPostIds = [mostSimilar.Id, unpublished.Id, leastSimilar.Id],
        });

        var page = await harness.PageQuery.GetAsync(blogPost.Id);

        page.ShouldNotBeNull();
        page.SimilarBlogPosts.Select(s => s.Title).ShouldBe(["Most Similar", "Least Similar"]);
        var summary = page.SimilarBlogPosts[0];
        summary.Id.ShouldBe(mostSimilar.Id);
        summary.Slug.ShouldBe(mostSimilar.Slug);
        summary.Tags.ShouldBe(["tag"]);
        summary.ShortDescription.ShouldBe(mostSimilar.ShortDescription);
        summary.ReadingTimeInMinutes.ShouldBe(mostSimilar.ReadingTimeInMinutes);
    }

    [Fact]
    public async Task ShouldReadSimilarBlogPostsStoredUnderBlogPostId()
    {
        if (!harness.CanStoreSimilarBlogPostUnderBlogPostId)
        {
            Assert.Skip("Document ids are global across collections for this provider.");
        }

        var blogPost = await StoreAsync(new BlogPostBuilder().Build());
        var similar = await StoreAsync(new BlogPostBuilder().WithTitle("Similar").IsPublished().Build());
        await harness.StoreAsync(new SimilarBlogPost { Id = blogPost.Id, SimilarBlogPostIds = [similar.Id] });

        var page = await harness.PageQuery.GetAsync(blogPost.Id);

        page!.SimilarBlogPosts.ShouldHaveSingleItem().Title.ShouldBe("Similar");
    }

    [Fact]
    public async Task ShouldNotReturnVersionAsBlogPost()
    {
        var blogPost = await StoreAsync(new BlogPostBuilder().Build());
        await ReviseAsync(blogPost.Id, "Changed");
        var versionId = BlogPostVersion.CreateId(blogPost.Id, 1);

        (await harness.PageQuery.GetAsync(versionId)).ShouldBeNull();
        (await Sut.GetAsync(versionId)).ShouldBeNull();
    }

    [Fact]
    public async Task ShouldListPublishedBlogPostsNewestFirstInPages()
    {
        await StoreAsync(new BlogPostBuilder().WithTitle("Old").IsPublished().WithUpdatedDate(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Newest").IsPublished().WithUpdatedDate(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Middle").IsPublished().WithUpdatedDate(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Draft").IsPublished(false).Build());

        var firstPage = await harness.ListQuery.GetPublishedAsync(1, 2);
        var secondPage = await harness.ListQuery.GetPublishedAsync(2, 2);
        var all = await harness.ListQuery.GetPublishedAsync();

        firstPage.Select(s => s.Title).ShouldBe(["Newest", "Middle"]);
        firstPage.IsLastPage.ShouldBeFalse();
        secondPage.IsLastPage.ShouldBeTrue();
        secondPage.Select(s => s.Title).ShouldBe(["Old"]);
        all.Select(s => s.Title).ShouldBe(["Newest", "Middle", "Old"]);
    }

    [Fact]
    public async Task ShouldListPublishedBlogPostsByTag()
    {
        await StoreAsync(new BlogPostBuilder().WithTitle("Tagged").WithTags("dotnet", "csharp").IsPublished().Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Other").WithTags("java").IsPublished().Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Tagged Draft").WithTags("dotnet").IsPublished(false).Build());

        var blogPosts = await harness.ListQuery.GetPublishedByTagAsync("dotnet");

        blogPosts.ShouldHaveSingleItem().Title.ShouldBe("Tagged");
    }

    [Fact]
    public async Task ShouldSearchPublishedBlogPostsByTitleIgnoringCaseOrByTag()
    {
        await StoreAsync(new BlogPostBuilder().WithTitle("Hello World").IsPublished().WithUpdatedDate(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Tagged").WithTags("hello").IsPublished().WithUpdatedDate(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Hello Draft").IsPublished(false).Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Unrelated").IsPublished().Build());

        var byTerm = await harness.ListQuery.SearchPublishedAsync("hello");
        var byUpperCaseTerm = await harness.ListQuery.SearchPublishedAsync("WORLD");

        byTerm.Select(s => s.Title).ShouldBe(["Hello World", "Tagged"]);
        byUpperCaseTerm.ShouldHaveSingleItem().Title.ShouldBe("Hello World");
    }

    [Fact]
    public async Task ShouldListDraftsWithScheduleAndAuthor()
    {
        var scheduledDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        await StoreAsync(new BlogPostBuilder().WithTitle("Scheduled").IsPublished(false).WithScheduledPublishDate(scheduledDate).WithAuthorName("Author").Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Published").IsPublished().Build());

        var drafts = await harness.ListQuery.GetDraftsAsync();

        var draft = drafts.ShouldHaveSingleItem();
        draft.Title.ShouldBe("Scheduled");
        draft.IsScheduled.ShouldBeTrue();
        draft.ScheduledPublishDate.ShouldBe(scheduledDate);
        draft.AuthorName.ShouldBe("Author");
        draft.Slug.ShouldBe("scheduled");
    }

    [Fact]
    public async Task ShouldListBlogPostsByIds()
    {
        var first = await StoreAsync(new BlogPostBuilder().WithTitle("First").IsPublished().Build());
        await StoreAsync(new BlogPostBuilder().WithTitle("Second").IsPublished().Build());
        var third = await StoreAsync(new BlogPostBuilder().WithTitle("Third").IsPublished().Build());

        var blogPosts = await harness.ListQuery.GetByIdsAsync([first.Id, third.Id]);

        blogPosts.Select(s => s.Title).ShouldBe(["First", "Third"], ignoreOrder: true);
    }

    protected abstract Task<IBlogPostPersistenceHarness> CreateHarnessAsync();

    private async Task<BlogPost> StoreAsync(BlogPost blogPost)
    {
        await harness.StoreAsync(blogPost);
        return blogPost;
    }

    private ValueTask ReviseAsync(string blogPostId, string title)
    {
        var changes = new BlogPostBuilder().WithTitle(title).Build();
        return Sut.ReviseAsync(blogPostId, (post, latest) => post.Revise(changes, latest));
    }
}
