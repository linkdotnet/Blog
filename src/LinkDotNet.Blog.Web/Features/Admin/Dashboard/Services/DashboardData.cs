namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public class DashboardData
{
    public int TotalAmountOfUsers { get; init; }

    public int AmountOfUsersLast30Days { get; init; }

    public int TotalPageClicks { get; init; }

    public int PageClicksLast30Days { get; init; }

    public int TotalAboutMeClicks { get; init; }

    public int AboutMeClicksLast30Days { get; init; }
}
