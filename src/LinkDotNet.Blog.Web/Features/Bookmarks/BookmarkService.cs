using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public class BookmarkService : IBookmarkService
{
    private const string Key = "bookmarks";
    private readonly ILocalStorageService localStorageService;
    private Task<HashSet<string>>? bookmarks;

    public BookmarkService(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task<bool> IsBookmarked(string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        return (await GetBookmarksAsync()).Contains(postId);
    }

    public async Task<IReadOnlyList<string>> GetBookmarkedPostIds() => [.. await GetBookmarksAsync()];

    public async Task SetBookmark(string postId, bool isBookmarked)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);

        // Re-read so a bookmark added in another tab since this circuit loaded is not overwritten
        var current = await ReadBookmarksAsync();
        if (isBookmarked)
        {
            current.Add(postId);
        }
        else
        {
            current.Remove(postId);
        }

        bookmarks = Task.FromResult(current);
        await localStorageService.SetItemAsync(Key, current);
    }

    private Task<HashSet<string>> GetBookmarksAsync() => bookmarks ??= ReadBookmarksAsync();

    private async Task<HashSet<string>> ReadBookmarksAsync()
    {
        try
        {
            return await localStorageService.GetItemAsync<HashSet<string>>(Key);
        }
        catch (KeyNotFoundException)
        {
            return [];
        }
    }
}
