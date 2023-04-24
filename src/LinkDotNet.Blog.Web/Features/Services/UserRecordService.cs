using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features.Services;

public class UserRecordService : IUserRecordService
{
    private readonly IRepository<UserRecord> userRecordRepository;
    private readonly NavigationManager navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ILocalStorageService localStorageService;
    private readonly ILogger<UserRecordService> logger;

    public UserRecordService(
        IRepository<UserRecord> userRecordRepository,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorageService,
        ILogger<UserRecordService> logger)
    {
        this.userRecordRepository = userRecordRepository;
        this.navigationManager = navigationManager;
        this.authenticationStateProvider = authenticationStateProvider;
        this.localStorageService = localStorageService;
        this.logger = logger;
    }

    public async ValueTask StoreUserRecordAsync()
    {
        try
        {
            await GetAndStoreUserRecordAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while storing user record service: {Exception}", e);
        }
    }

    private async ValueTask GetAndStoreUserRecordAsync()
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
            DateClicked = DateOnly.FromDateTime(DateTime.UtcNow),
            UrlClicked = url,
        };

        await userRecordRepository.StoreAsync(record);
    }

    private async ValueTask<int> GetIdentifierHashAsync()
    {
        if (await HasKeyAsync())
        {
            var key = await localStorageService.GetItemAsync<Guid>("user");
            return key.GetHashCode();
        }

        var id = Guid.NewGuid();
        await localStorageService.SetItemAsync("user", id);
        return id.GetHashCode();
    }

    private async ValueTask<bool> HasKeyAsync()
    {
        try
        {
            return await localStorageService.ContainKeyAsync("user");
        }
        catch (Exception e)
        {
            logger.LogError("Couldn't obtain key for user: {Exception}", e);
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
