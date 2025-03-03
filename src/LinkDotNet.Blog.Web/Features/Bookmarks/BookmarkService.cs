using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.EntityFrameworkCore;

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

    public async Task ToggleBookmark(string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        await InitializeIfNotExists();

        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        if (await IsBookmarked(postId))
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
        if (!(await localStorageService.ContainKeyAsync("bookmarks")))
        {
            await localStorageService.SetItemAsync("bookmarks", new List<string>());
        }
    }
}
