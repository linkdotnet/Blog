using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LinkDotNet.Blog.Web.Shared.Services;

public class UserRecordService : IUserRecordService
{
    private readonly IRepository<UserRecord> userRecordRepository;
    private readonly NavigationManager navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ILocalStorageService localStorageService;

    public UserRecordService(
        IRepository<UserRecord> userRecordRepository,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorageService)
    {
        this.userRecordRepository = userRecordRepository;
        this.navigationManager = navigationManager;
        this.authenticationStateProvider = authenticationStateProvider;
        this.localStorageService = localStorageService;
    }

    public async Task StoreUserRecordAsync()
    {
        try
        {
            await GetAndStoreUserRecordAsync();
        }
        catch (Exception e)
        {
            Console.Write($"Exception: {e}");
        }
    }

    private async Task GetAndStoreUserRecordAsync()
    {
        var userIdentity = (await authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userIdentity == null || userIdentity.IsAuthenticated)
        {
            return;
        }

        var identifierHash = await GetIdentifierHashAsync();

        var url = GetClickedUrl();

        var record = new UserRecord
        {
            UserIdentifierHash = identifierHash,
            DateTimeUtcClicked = DateTime.UtcNow,
            UrlClicked = url,
        };

        await userRecordRepository.StoreAsync(record);
    }

    private async Task<int> GetIdentifierHashAsync()
    {
        var hasKey = await TryGetKey();
        if (hasKey)
        {
            var key = await localStorageService.GetItemAsync<Guid>("user");
            return key.GetHashCode();
        }

        var id = Guid.NewGuid();
        await localStorageService.SetItemAsync("user", id);
        return id.GetHashCode();
    }

    private async Task<bool> TryGetKey()
    {
        try
        {
            var hasKey = await localStorageService.ContainKeyAsync("user");
            return hasKey;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't obtain key: \"user\": {e}");
            return false;
        }
    }

    private string GetClickedUrl()
    {
        var basePath = navigationManager.ToBaseRelativePath(navigationManager.Uri);

        if (string.IsNullOrEmpty(basePath))
        {
            return string.Empty;
        }

        var queryIndex = basePath.IndexOf('?');
        return queryIndex >= 0 ? basePath[..queryIndex] : basePath;
    }
}
