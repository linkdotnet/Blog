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
    public async Task ShouldReadStorageOnceForSeveralChecks()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1"]);

        await bookmarkService.IsBookmarked("1");
        await bookmarkService.IsBookmarked("2");
        await bookmarkService.GetBookmarkedPostIds();

        await localStorageService.Received(1).GetItemAsync<HashSet<string>>("bookmarks");
    }

    [Fact]
    public async Task ShouldKeepBookmarksFromOtherTabsWhenSetting()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(_ => ["1"], _ => ["1", "2"]);
        await bookmarkService.IsBookmarked("1");

        await bookmarkService.SetBookmark("3", true);

        await localStorageService.Received(1).SetItemAsync("bookmarks", Arg.Is<HashSet<string>>(s => s.SetEquals(new[] { "1", "2", "3" })));
        (await bookmarkService.IsBookmarked("2")).ShouldBeTrue();
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
