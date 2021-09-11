using FluentAssertions;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared.Services;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web
{
    public class StartupTests
    {
        [Fact]
        public void ShouldSetServices()
        {
            var host = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();

            host.Services.GetRequiredService<IRepository<BlogPost>>().Should().NotBeNull();
            host.Services.GetRequiredService<ISortOrderCalculator>().Should().NotBeNull();
            host.Services.GetRequiredService<IDashboardService>().Should().NotBeNull();
            host.Services.GetRequiredService<IUserRecordService>().Should().NotBeNull();
            host.Services.GetRequiredService<AppConfiguration>().Should().NotBeNull();
        }
    }
}