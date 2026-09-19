using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed partial class UserRecordService : IUserRecordService
{
    private readonly IAnalyticsRepository analyticsRepository;
    private readonly NavigationManager navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<UserRecordService> logger;

    public UserRecordService(
        IAnalyticsRepository analyticsRepository,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        TimeProvider timeProvider,
        ILogger<UserRecordService> logger)
    {
        this.analyticsRepository = analyticsRepository;
        this.navigationManager = navigationManager;
        this.authenticationStateProvider = authenticationStateProvider;
        this.timeProvider = timeProvider;
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
            LogUserRecordError(e);
        }
    }

    private async ValueTask GetAndStoreUserRecordAsync()
    {
        var userIdentity = (await authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userIdentity is { IsAuthenticated: true })
        {
            return;
        }

        var url = GetClickedUrl();

        var record = new UserRecord
        {
            DateClicked = DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime),
            UrlClicked = url,
        };

        await analyticsRepository.StoreUserRecordAsync(record);
    }

    private string GetClickedUrl()
    {
        var basePath = navigationManager.ToBaseRelativePath(navigationManager.Uri);

        if (string.IsNullOrEmpty(basePath))
        {
            return string.Empty;
        }

        var queryIndex = basePath.IndexOf('?', StringComparison.OrdinalIgnoreCase);
        return queryIndex >= 0 ? basePath[..queryIndex] : basePath;
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Error while storing user record service.")]
    private partial void LogUserRecordError(Exception exception);
}
