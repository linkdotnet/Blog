using FluentAssertions;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions
{
    public class SqliteRegistrationExtensionsTests
    {
        [Fact]
        public void ShouldGetValidRepository()
        {
            var serviceCollection = new ServiceCollection();
            var appConfig = new AppConfiguration
            {
                ConnectionString = "Filename=:memory:",
            };
            serviceCollection.AddScoped(_ => appConfig);

            serviceCollection.UseSqliteAsStorageProvider();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<IRepository<BlogPost>>().Should().NotBeNull();
            serviceProvider.GetService<IRepository<Skill>>().Should().NotBeNull();
        }
    }
}