using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests
{
    public class SmokeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;

        public SmokeTest(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("PersistenceProvider", "InMemory");
            });
        }

        [Fact]
        public async Task ShouldBootUpApplication()
        {
            var client = factory.CreateClient();

            var result = await client.GetAsync("/");

            result.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
