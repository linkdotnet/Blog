using System;
using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions;

public class StorageProviderExtensionsTests
{
    [Theory]
    [InlineData("SqlServer")]
    [InlineData("Sqlite")]
    [InlineData("RavenDb")]
    [InlineData("InMemory")]
    [InlineData("MySql")]
    public void ShouldRegisterPersistenceProvider(string persistenceKey)
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns(persistenceKey);

        collection.AddStorageProvider(config);

        var enumerable = collection.Select(c => c.ServiceType).ToList();
        enumerable.Should().Contain(typeof(IRepository<>));
    }

    [Fact]
    public void ShouldThrowExceptionWhenNotKnown()
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns("not known");

        Action act = () => collection.AddStorageProvider(config);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ShouldHaveCacheRepositoryOnlyForBlogPosts()
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns("Sqlite");
        collection.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder()
            .WithConnectionString("Filename=:memory:")
            .Build()));
        
        collection.AddLogging();

        collection.AddStorageProvider(config);

        var serviceProvider = collection.BuildServiceProvider();
        serviceProvider.GetService<IRepository<BlogPost>>().Should().BeOfType<CachedRepository<BlogPost>>();
        serviceProvider.GetService<IRepository<Skill>>().Should().NotBeOfType<CachedRepository<BlogPost>>();
    }
}
