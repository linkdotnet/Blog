using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.DummyData;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.DummyData;

public class DummyDataExtensionsTests
{
    [Fact]
    public void ShouldReplaceAllPersistenceRegistrationsOfTheConfiguredProvider()
    {
        var services = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns("MongoDB");
        services.AddStorageProvider(config);

        services.UseDummyData();

        services.Count(s => s.ServiceType == typeof(IRepository<>)).ShouldBe(1);
        services.Count(s => s.ServiceType == typeof(IBlogPostRepository)).ShouldBe(1);
        services.Count(s => s.ServiceType == typeof(IBlogPostPageQuery)).ShouldBe(1);
        services.Count(s => s.ServiceType == typeof(IBlogPostListQuery)).ShouldBe(1);
        services.ShouldNotContain(s => s.ServiceType == typeof(IRepository<BlogPost>));
    }
}
