using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace LinkDotNet.Blog.Web.Shared.Services
{
    public interface ILocalStorageService
    {
        Task<bool> ContainKeyAsync(string key);

        Task<T> GetItemAsync<T>(string key);

        Task SetItemAsync<T>(string key, T value);
    }


    public class LocalStorageService : ILocalStorageService
    {
        private readonly ProtectedLocalStorage localStorage;

        public LocalStorageService(ProtectedLocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task<bool> ContainKeyAsync(string key)
        {
            return (await localStorage.GetAsync<object>(key)).Success;
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            return (await localStorage.GetAsync<T>(key)).Value;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            await localStorage.SetAsync(key, value);
        }
    }
}