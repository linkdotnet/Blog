using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LinkDotNet.Blog.Web.Shared
{
    public interface IUserRecordService
    {
        Task StoreUserRecordAsync();
    }

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
                Trace.Write($"Exception: {e}");
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
            var hasKey = await localStorageService.ContainKeyAsync("user");
            if (hasKey)
            {
                var key = await localStorageService.GetItemAsync<Guid>("user");
                return key.GetHashCode();
            }

            var id = Guid.NewGuid();
            await localStorageService.SetItemAsync("user", id);
            return id.GetHashCode();
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
}