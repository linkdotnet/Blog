using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Services;
using NSubstitute.ReceivedExtensions;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Bookmarks;

public class BookmarkServiceTests
{
    private ILocalStorageService localStorageService;
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
            .GetItemAsync<IReadOnlyList<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var ids = await bookmarkService.GetBookmarkedPostIds();
        
        ids.ShouldBe(["1", "2", "3"]);
        ids.ShouldBeUnique();
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenNoBookmarks()
    {
        localStorageService
            .GetItemAsync<IReadOnlyList<string>>("bookmarks")
            .Returns(x => []);
        
        var ids = await bookmarkService.GetBookmarkedPostIds();
        
        ids.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnTrueIfBookmarked()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var isBookmarked = await bookmarkService.IsBookMarked("1");
        
        isBookmarked.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseIfNoBookmarked()
    {
        localStorageService
            .GetItemAsync<HashSet<string>>("bookmarks")
            .Returns(x => ["1", "2", "3"]);

        var isBookmarked = await bookmarkService.IsBookMarked("4");
        
        isBookmarked.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldThrowArgumentExceptionWhenIdIsEmptyOrNull()
    {
        var id = string.Empty;

        await bookmarkService.ToggleBookmark(id)
            .ShouldThrowAsync<ArgumentException>();

        await bookmarkService.IsBookMarked(id)
            .ShouldThrowAsync<ArgumentException>();
    }
}
