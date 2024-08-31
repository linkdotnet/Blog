using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions;

public class RavenDbRegistrationExtensionsTests
{
    [Fact]
    public void ShouldGetValidRepository()
    {
        var serviceCollection = new ServiceCollection();
        var appConfig = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogName("Blog")
            .WithConnectionString("http://localhost")
            .Build());
        serviceCollection.AddScoped(_ => appConfig);

        serviceCollection.UseRavenDbAsStorageProvider();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.GetService<IRepository<BlogPost>>().Should().NotBeNull();
        serviceProvider.GetService<IRepository<Skill>>().Should().NotBeNull();
    }
}
