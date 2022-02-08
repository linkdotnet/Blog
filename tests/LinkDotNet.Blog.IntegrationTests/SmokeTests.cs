using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LinkDotNet.Blog.IntegrationTests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("PersistenceProvider", PersistenceProvider.InMemory.Key);
        });
    }

    [Fact]
    public async Task ShouldBootUpApplication()
    {
        var client = factory.CreateClient();

        var result = await client.GetAsync("/");

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldBootUpWithSqlDatabase()
    {
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("PersistenceProvider", PersistenceProvider.SqliteServer.Key);
            builder.UseSetting("ConnectionString", "DataSource=file::memory:?cache=shared");
        }).CreateClient();

        var result = await client.GetAsync("/");

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldAllowDotsForTagSearch()
    {
        var client = factory.CreateClient();

        var result = await client.GetAsync("/searchByTag/.NET5");

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldAllowDotsForFreeTextSearch()
    {
        var client = factory.CreateClient();

        var result = await client.GetAsync("/searchByTag/.NET5");

        result.IsSuccessStatusCode.Should().BeTrue();
    }
}