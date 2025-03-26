using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    public async ValueTask<bool> ContainsKeyAsync(string key)
    {
        try
        {
            return (await localStorage.GetAsync<object>(key)).Success;
        }
        catch (CryptographicException)
        {
            await localStorage.DeleteAsync(key);
            return false;
        }
    }

    public async ValueTask<T> GetItemAsync<T>(string key)
    {
        try
        {
            var result = await localStorage.GetAsync<T>(key);
            if (!result.Success)
            {
                throw new KeyNotFoundException($"Key {key} not found");
            }
            return result.Value!;
        }
        catch (CryptographicException)
        {
            await localStorage.DeleteAsync(key);
            throw new KeyNotFoundException($"Key {key} was invalid and has been removed");
        }
    }

    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        try
        {
            await localStorage.SetAsync(key, value!);
        }
        catch (CryptographicException)
        {
            await localStorage.DeleteAsync(key);
            throw new InvalidOperationException($"Could not set value for key {key}. The key has been removed.");
        }
    }
}
