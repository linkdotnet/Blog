using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services;

public interface ILocalStorageService
{
    Task<bool> ContainKeyAsync(string key);

    Task<T> GetItemAsync<T>(string key);

    Task SetItemAsync<T>(string key, T value);
}
