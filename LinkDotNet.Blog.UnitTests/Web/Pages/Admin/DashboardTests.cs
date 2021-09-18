using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared.Admin.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Pages.Admin
{
    public class DashboardTests : TestContext
    {
        [Fact]
        public void ShouldNotShowAboutMeStatisticsWhenDisabled()
        {
            var dashboardService = new Mock<IDashboardService>();
            this.AddTestAuthorization().SetAuthorized("test");
            Services.AddScoped(_ => CreateAppConfiguration(false));
            Services.AddScoped(_ => dashboardService.Object);
            Services.AddScoped(_ => new Mock<IRepository<BlogPost>>().Object);
            dashboardService.Setup(d => d.GetDashboardDataAsync())
                .ReturnsAsync(new DashboardData());

            var cut = RenderComponent<Dashboard>();

            cut.FindComponents<DashboardCard>()
                .Any(c => c.Instance.Text == "About Me:")
                .Should()
                .BeFalse();
        }

        private static AppConfiguration CreateAppConfiguration(bool aboutMeEnabled)
        {
            return new AppConfiguration
            {
                ProfileInformation = aboutMeEnabled ? new ProfileInformation() : null,
            };
        }
    }
}