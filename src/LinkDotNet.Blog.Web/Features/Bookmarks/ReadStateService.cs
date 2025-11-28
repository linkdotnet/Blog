using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.Web.Features.Bookmarks;

public class ReadStateService : IReadStateService
{
    private const string StorageKey = "readPosts";
    private readonly ILocalStorageService localStorageService;

    public ReadStateService(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task<bool> IsRead(string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        await InitializeIfNotExists();
        var readPosts = await localStorageService.GetItemAsync<HashSet<string>>(StorageKey);

        return readPosts.Contains(postId);
    }

    public async Task<IReadOnlyCollection<string>> GetReadPostIds()
    {
        await InitializeIfNotExists();
        return await localStorageService.GetItemAsync<IReadOnlyCollection<string>>(StorageKey);
    }

    public async Task MarkAsRead(string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId);
        await InitializeIfNotExists();

        var readPosts = await localStorageService.GetItemAsync<HashSet<string>>(StorageKey);
        readPosts.Add(postId);

        await localStorageService.SetItemAsync(StorageKey, readPosts);
    }

    private async Task InitializeIfNotExists()
    {
        if (!await localStorageService.ContainsKeyAsync(StorageKey))
        {
            await localStorageService.SetItemAsync(StorageKey, Array.Empty<string>());
        }
    }
}
