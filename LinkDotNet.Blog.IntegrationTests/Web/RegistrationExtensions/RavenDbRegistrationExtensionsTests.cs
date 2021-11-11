using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions;

public class RavenDbRegistrationExtensionsTests
{
    [Fact]
    public void ShouldGetValidRepository()
    {
        var serviceCollection = new ServiceCollection();
        var appConfig = new AppConfiguration
        {
            ConnectionString = "http://localhost",
            DatabaseName = "Blog",
        };
        serviceCollection.AddScoped(_ => appConfig);

        serviceCollection.UseRavenDbAsStorageProvider();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.GetService<IRepository<BlogPost>>().Should().NotBeNull();
        serviceProvider.GetService<IRepository<Skill>>().Should().NotBeNull();
    }
}
