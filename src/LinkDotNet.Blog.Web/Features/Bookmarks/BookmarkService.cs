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
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    public BookmarkService(ILocalStorageService localStorageService, IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.localStorageService = localStorageService;
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<bool> IsBookMarked(string postId)
    {
        ArgumentNullException.ThrowIfNull(postId);
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        return bookmarks.Contains(postId);
    }

    public async Task<IReadOnlyList<BlogPost>> GetBookmarkedPosts()
    {
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");
        var context = await dbContextFactory.CreateDbContextAsync();

        var results = await context.BlogPosts.Where(p => bookmarks.Contains(p.Id)).ToListAsync();

        await context.DisposeAsync();

        return results;
    }

    public async Task BookMarkPost(BlogPost post)
    {
        ArgumentNullException.ThrowIfNull(post);
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        bookmarks.Add(post.Id);

        await localStorageService.SetItemAsync("bookmarks", bookmarks);
    }

    public async Task RemovePost(string postId)
    {
        ArgumentNullException.ThrowIfNull(postId);
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        bookmarks.Remove(postId);

        await localStorageService.SetItemAsync("bookmarks", bookmarks);
    }

    private async Task InitializeIfNotExists()
    {
        if (!(await localStorageService.ContainKeyAsync("bookmarks")))
            await localStorageService.SetItemAsync("bookmarks", new List<string>());
    }
}
