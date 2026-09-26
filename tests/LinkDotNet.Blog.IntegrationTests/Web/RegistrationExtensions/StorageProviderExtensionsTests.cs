using System;
using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
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
    [InlineData("MongoDB")]
    [InlineData("MySql")]
    [InlineData("PostgreSql")]
    public void ShouldRegisterPersistenceProvider(string persistenceKey)
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns(persistenceKey);

        collection.AddStorageProvider(config);

        var enumerable = collection.Select(c => c.ServiceType).ToList();
        enumerable.ShouldContain(typeof(IRepository<>));
        enumerable.ShouldContain(typeof(IBlogPostRepository));
        enumerable.ShouldContain(typeof(IBlogPostPageQuery));
        enumerable.ShouldContain(typeof(IBlogPostListQuery));
    }

    [Theory]
    [InlineData("SqlServer")]
    [InlineData("Sqlite")]
    [InlineData("RavenDb")]
    [InlineData("MongoDB")]
    [InlineData("MySql")]
    [InlineData("PostgreSql")]
    public void ShouldResolveBlogPostVersionServiceForEveryProvider(string persistenceKey)
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns(persistenceKey);
        collection.AddSingleton(Options.Create(new ApplicationConfigurationBuilder().Build()));
        collection.AddLogging();
        collection.AddScoped<IBlogPostVersionService, BlogPostVersionService>();
        collection.AddStorageProvider(config);

        Action act = () => collection.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        act.ShouldNotThrow();
    }

    [Fact]
    public void ShouldThrowExceptionWhenNotKnown()
    {
        var collection = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();
        config["PersistenceProvider"].Returns("not known");

        Action act = () => collection.AddStorageProvider(config);

        act.ShouldThrow<Exception>();
    }

    [Fact]
    public void ShouldDecorateBlogPostPersistenceWithCaching()
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
        using var scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetService<IBlogPostPageQuery>().ShouldBeOfType<CachedBlogPostPageQuery>();
        scope.ServiceProvider.GetService<IBlogPostRepository>().ShouldBeOfType<CacheInvalidatingBlogPostRepository>();
    }
}
