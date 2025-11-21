using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface ILocalStorageService
{
    ValueTask<bool> ContainsKeyAsync(string key);

    ValueTask<T> GetItemAsync<T>(string key);

    ValueTask SetItemAsync<T>(string key, T value);

    ValueTask RemoveItemAsync(string key);
}
