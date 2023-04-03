using System;
using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["PersistenceProvider"])
            .Returns(persistenceKey);

        collection.AddStorageProvider(config.Object);

        var enumerable = collection.Select(c => c.ServiceType).ToList();
        enumerable.Should().Contain(typeof(IRepository<>));
    }

    [Fact]
    public void ShouldThrowExceptionWhenNotKnown()
    {
        var collection = new ServiceCollection();
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["PersistenceProvider"])
            .Returns("not known");

        Action act = () => collection.AddStorageProvider(config.Object);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ShouldHaveCacheRepositoryOnlyForBlogPosts()
    {
        var collection = new ServiceCollection();
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["PersistenceProvider"])
            .Returns("Sqlite");
        collection.AddScoped(_ => new AppConfiguration { ConnectionString = "Filename=:memory:" });

        collection.AddStorageProvider(config.Object);

        var serviceProvider = collection.BuildServiceProvider();
        serviceProvider.GetService<IRepository<BlogPost>>().Should().BeOfType<CachedRepository<BlogPost>>();
        serviceProvider.GetService<IRepository<Skill>>().Should().NotBeOfType<CachedRepository<BlogPost>>();
    }
}
