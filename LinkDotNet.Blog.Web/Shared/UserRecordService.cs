using System;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace LinkDotNet.Blog.Web.Shared
{
    public interface IUserRecordService
    {
        Task StoreUserRecordAsync();
    }

    public class UserRecordService : IUserRecordService
    {
        private readonly IRepository<UserRecord> userRecordRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NavigationManager navigationManager;

        public UserRecordService(
            IRepository<UserRecord> userRecordRepository,
            IHttpContextAccessor httpContextAccessor,
            NavigationManager navigationManager)
        {
            this.userRecordRepository = userRecordRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.navigationManager = navigationManager;
        }

        public async Task StoreUserRecordAsync()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var ipHash = httpContext.Connection.RemoteIpAddress?.GetHashCode() ?? 0;

            var record = new UserRecord
            {
                IpHash = ipHash,
                DateTimeUtcClicked = DateTime.UtcNow,
                UrlClicked = navigationManager.ToBaseRelativePath(navigationManager.Uri),
            };

            await userRecordRepository.StoreAsync(record);
        }
    }
}