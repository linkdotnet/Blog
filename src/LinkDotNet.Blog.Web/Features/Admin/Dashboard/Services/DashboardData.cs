namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public class DashboardData
{
    public int TotalAmountOfUsers { get; set; }

    public int AmountOfUsersLast30Days { get; set; }

    public int TotalPageClicks { get; set; }

    public int PageClicksLast30Days { get; set; }

    public int TotalAboutMeClicks { get; set; }

    public int AboutMeClicksLast30Days { get; set; }
}
