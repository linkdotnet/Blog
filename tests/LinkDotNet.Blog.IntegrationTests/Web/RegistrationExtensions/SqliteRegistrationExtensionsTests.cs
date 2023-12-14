using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions;

public class SqliteRegistrationExtensionsTests
{
    [Fact]
    public void ShouldGetValidRepository()
    {
        var serviceCollection = new ServiceCollection();
        var appConfig = Options.Create(new ApplicationConfiguration()
        {
            ConnectionString = "Filename=:memory:",
        });
        serviceCollection.AddScoped(_ => appConfig);
        serviceCollection.AddLogging();

        serviceCollection.UseSqliteAsStorageProvider();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.GetService<IRepository<BlogPost>>().Should().NotBeNull();
        serviceProvider.GetService<IRepository<Skill>>().Should().NotBeNull();
    }
}
