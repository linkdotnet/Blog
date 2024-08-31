using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class LocalStorageService : ILocalStorageService
{
    private readonly ProtectedLocalStorage localStorage;

    public LocalStorageService(ProtectedLocalStorage localStorage)
    {
        this.localStorage = localStorage;
    }

    public async ValueTask<bool> ContainKeyAsync(string key) => (await localStorage.GetAsync<object>(key)).Success;

    public async ValueTask<T> GetItemAsync<T>(string key) => (await localStorage.GetAsync<T>(key)).Value
                                                             ?? throw new KeyNotFoundException($"Key {key} not found");

    public async ValueTask SetItemAsync<T>(string key, T value) => await localStorage.SetAsync(key, value!);
}
