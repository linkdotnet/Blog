using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger logger;

        public UserRecordService(
            IRepository<UserRecord> userRecordRepository,
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService,
            ILogger logger)
        {
            this.userRecordRepository = userRecordRepository;
            this.navigationManager = navigationManager;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
            this.logger = logger;
        }

        public async Task StoreUserRecordAsync()
        {
            try
            {
                await GetAndStoreUSerRecordAsync();
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e, "Couldn't write user record");
            }
        }

        private async Task GetAndStoreUSerRecordAsync()
        {
            var userIdentity = (await authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
            if (userIdentity == null || userIdentity.IsAuthenticated)
            {
                return;
            }

            var identifierHash = await GetIdentifierHashAsync();

            var record = new UserRecord
            {
                UserIdentifierHash = identifierHash,
                DateTimeUtcClicked = DateTime.UtcNow,
                UrlClicked = navigationManager.ToBaseRelativePath(navigationManager.Uri),
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
    }
}