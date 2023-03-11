using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class UserRecordService : IUserRecordService
{
    private readonly IRepository<UserRecord> userRecordRepository;
    private readonly NavigationManager navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ILogger<UserRecordService> logger;

    public UserRecordService(
        IRepository<UserRecord> userRecordRepository,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        ILogger<UserRecordService> logger)
    {
        this.userRecordRepository = userRecordRepository;
        this.navigationManager = navigationManager;
        this.authenticationStateProvider = authenticationStateProvider;
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

        var url = GetClickedUrl();

        var record = new UserRecord
        {
            DateClicked = DateOnly.FromDateTime(DateTime.UtcNow),
            UrlClicked = url,
        };

        await userRecordRepository.StoreAsync(record);
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
