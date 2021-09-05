using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Pages.Admin
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardDataAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IRepository<UserRecord> userRecordRepository;

        public DashboardService(
            IRepository<UserRecord> userRecordRepository)
        {
            this.userRecordRepository = userRecordRepository;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            var records = (await userRecordRepository.GetAllAsync()).ToList();
            var users = records.GroupBy(r => r.IpHash).Count();
            var users30Days = records
                .Where(r => r.DateTimeUtcClicked >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(r => r.IpHash).Count();

            var clicks = records.Count;
            var clicks30Days = records.Count(r => r.DateTimeUtcClicked >= DateTime.UtcNow.AddDays(-30));

            var visitCount = GetPageVisitCount(records);

            return new DashboardData
            {
                TotalAmountOfUsers = users,
                AmountOfUsersLast30Days = users30Days,
                TotalPageClicks = clicks,
                PageClicksLast30Days = clicks30Days,
                BlogPostVisitCount = visitCount,
            };
        }

        private static IOrderedEnumerable<KeyValuePair<string, int>> GetPageVisitCount(IEnumerable<UserRecord> records)
        {
            return records
                .Where(u => u.UrlClicked.Contains("blogPost"))
                .GroupBy(u => u.UrlClicked)
                .ToDictionary(k => k.Key, v => v.Count())
                .OrderByDescending(d => d.Value);
        }
    }
}