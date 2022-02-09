using System.Data.Common;
using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Dashboard;

public class DashboardPageTests : TestContext
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldShowAboutMeStatisticsAccordingToConfig(bool aboutMeEnabled)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(CreateInMemoryConnection())
            .Options;
        var dashboardService = new Mock<IDashboardService>();
        this.AddTestAuthorization().SetAuthorized("test");
        Services.AddScoped(_ => CreateAppConfiguration(aboutMeEnabled));
        Services.AddScoped(_ => dashboardService.Object);
        Services.AddScoped(_ => Mock.Of<IRepository<BlogPost>>());
        Services.AddScoped(_ => new BlogDbContext(options));
        dashboardService.Setup(d => d.GetDashboardDataAsync())
            .ReturnsAsync(new DashboardData());

        var cut = RenderComponent<DashboardPage>();

        cut.FindComponents<DashboardCard>()
            .Any(c => c.Instance.Text == "About Me:")
            .Should()
            .Be(aboutMeEnabled);
    }

    private static AppConfiguration CreateAppConfiguration(bool aboutMeEnabled)
    {
        return new AppConfiguration
        {
            ProfileInformation = aboutMeEnabled ? new ProfileInformation() : null,
        };
    }

    private static DbConnection CreateInMemoryConnection()
    {
        var connection = new SqliteConnection("Filename=:memory:");

        connection.Open();

        return connection;
    }
}