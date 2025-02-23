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

    public async Task<bool> IsBookMarked(string blogId)
    {
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        return bookmarks.Contains(blogId);
    }

    public async Task<IReadOnlyList<BlogPost>> GetBookmarkedPosts()
    {
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");
        var context = await dbContextFactory.CreateDbContextAsync();

        return await context.BlogPosts.Where(p => bookmarks.Contains(p.Id)).ToListAsync();
    }

    public async Task BookMarkPost(BlogPost post)
    {
        await InitializeIfNotExists();
        var bookmarks = await localStorageService.GetItemAsync<HashSet<string>>("bookmarks");

        bookmarks.Add(post.Id);

        await localStorageService.SetItemAsync("bookmarks", bookmarks);
    }

    public async Task RemovePost(string postId)
    {
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
