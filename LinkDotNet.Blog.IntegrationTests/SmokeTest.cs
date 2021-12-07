using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests
{
    public class SmokeTest
    {
        [Fact]
        public async Task ShouldBootUpApplication()
        {
            var application = new WebApplicationFactory<Program>().WithWebHostBuilder(b => { });
            var client = application.CreateClient();

            var result = await client.GetAsync("/");

            result.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
