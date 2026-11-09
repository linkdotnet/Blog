using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Bookmarks;

public class BookmarkServiceTests
{
    private readonly ILocalStorageService localStorageService;
    private readonly IBookmarkService bookmarkService;

    public BookmarkServiceTests()
    {
        localStorageService = Substitute.For<ILocalStorageService>();
        bookmarkService = new BookmarkService(localStorageService);
    }

    [Fact]
    public async Task ShouldReturnIds()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var ids = await bookmarkService.GetBookmarkedPostIds();
        
        ids.ShouldBe(["1", "2", "3"], ignoreOrder: true);
        ids.ShouldBeUnique();
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenNoBookmarks()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns<HashSet<string>>(_ => throw new KeyNotFoundException());
        
        var ids = await bookmarkService.GetBookmarkedPostIds();
        
        ids.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnTrueIfBookmarked()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var isBookmarked = await bookmarkService.IsBookmarked("1");
        
        isBookmarked.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseIfNoBookmarked()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var isBookmarked = await bookmarkService.IsBookmarked("4");
        
        isBookmarked.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldShareOnePendingReadBetweenConcurrentChecks()
    {
        var pendingRead = new TaskCompletionSource<HashSet<string>>();
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(_ => new ValueTask<HashSet<string>>(pendingRead.Task));

        var checks = new[] { bookmarkService.IsBookmarked("1"), bookmarkService.IsBookmarked("2"), bookmarkService.IsBookmarked("3") };
        pendingRead.SetResult(["2"]);

        (await Task.WhenAll(checks)).ShouldBe([false, true, false]);
        await localStorageService.Received(1).GetItemAsync<HashSet<string>>("bookmarks");
    }

    [Fact]
    public async Task ShouldReadAgainSoChangesFromOtherTabsShowUp()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(_ => [], _ => ["1"]);

        (await bookmarkService.IsBookmarked("1")).ShouldBeFalse();

        (await bookmarkService.IsBookmarked("1")).ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldKeepBookmarksFromOtherTabsWhenSetting()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(_ => ["1", "2"]);

        await bookmarkService.SetBookmark("3", true);

        await localStorageService.Received(1).SetItemAsync("bookmarks", Arg.Is<HashSet<string>>(s => s.SetEquals(new[] { "1", "2", "3" })));
    }

    [Fact]
    public async Task ShouldThrowArgumentExceptionWhenIdIsEmptyOrNull()
    {
        var id = string.Empty;

        await bookmarkService.SetBookmark(id, false)
            .ShouldThrowAsync<ArgumentException>();

        await bookmarkService.IsBookmarked(id)
            .ShouldThrowAsync<ArgumentException>();
    }
}
