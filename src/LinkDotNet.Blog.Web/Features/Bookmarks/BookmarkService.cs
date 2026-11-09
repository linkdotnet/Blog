using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public class BookmarkService : IBookmarkService
{
    private const string Key = "bookmarks";
    private readonly ILocalStorageService localStorageService;
    private Task<HashSet<string>>? inFlightRead;

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

        var current = await ReadBookmarksAsync();
        if (isBookmarked)
        {
            current.Add(postId);
        }
        else
        {
            current.Remove(postId);
        }

        await localStorageService.SetItemAsync(Key, current);
    }

    // Cards on one page ask at the same time; sharing only the pending read keeps later pages in sync with other tabs
    private async Task<HashSet<string>> GetBookmarksAsync()
    {
        var read = inFlightRead ??= ReadBookmarksAsync();
        try
        {
            return await read;
        }
        finally
        {
            if (inFlightRead == read)
            {
                inFlightRead = null;
            }
        }
    }

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
