using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardData> GetDashboardDataAsync();
}