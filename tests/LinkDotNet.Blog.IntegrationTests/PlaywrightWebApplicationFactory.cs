using System;
using System.Linq;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.DummyData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinkDotNet.Blog.IntegrationTests;

public sealed class PlaywrightWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? host;

    public string ServerAddress => ClientOptions.BaseAddress.ToString();

    public override IServiceProvider Services => host?.Services
                                                 ?? throw new InvalidOperationException("Create the Client first before retrieving instances from the container");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();

        builder = builder.ConfigureWebHost(b =>
        {
            b.UseSetting("Logging:LogLevel:Default", "Error");
            b.UseSetting("BlogName", "Integration Tests Blog");
            b.ConfigureServices(services =>
            {
                services.UseDummyData(new DummyDataOptions { NumberOfBlogPosts = 25 });
            });
            b.UseKestrel();
        });

        host?.Dispose();
        host = builder.Build();
        host.Start();

        var server = host!.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => new Uri(x))
            .Last();

        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            host?.Dispose();
        }

        base.Dispose(disposing);
    }
}
