using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public class BookmarkService : IBookmarkService
{
    private readonly ILocalStorageService localStorageService;

    public BookmarkService(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task<bool> IsBookmarked(string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        return bookmarks.Contains(postId);
    }

    public async Task<IReadOnlyList<string>> GetBookmarkedPostIds()
    {
        await InitializeIfNotExists();
        return await localStorageService.GetItemAsync<IReadOnlyList<string>>("bookmarks");
    }

    public async Task SetBookmark(string postId, bool isBookmarked)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        await InitializeIfNotExists();

        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        if (!isBookmarked)
        {
            bookmarks.Remove(postId);
        }
        else
        {
            bookmarks.Add(postId);
        }

        await localStorageService.SetItemAsync("bookmarks", bookmarks);

    }

    private async Task InitializeIfNotExists()
    {
        if (!(await localStorageService.ContainsKeyAsync("bookmarks")))
        {
            await localStorageService.SetItemAsync("bookmarks", new List<string>());
        }
    }
}
