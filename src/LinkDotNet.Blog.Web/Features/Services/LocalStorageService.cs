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

    public async ValueTask<bool> ContainKeyAsync(string key)
    {
        return (await localStorage.GetAsync<object>(key)).Success;
    }

    public async ValueTask<T> GetItemAsync<T>(string key)
    {
        return (await localStorage.GetAsync<T>(key)).Value;
    }

    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        await localStorage.SetAsync(key, value);
    }
}