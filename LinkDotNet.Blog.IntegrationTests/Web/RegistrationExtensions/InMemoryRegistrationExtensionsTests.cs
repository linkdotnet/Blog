using FluentAssertions;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions
{
    public class InMemoryRegistrationExtensionsTests
    {
        [Fact]
        public void ShouldGetValidRepository()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.UseInMemoryAsStorageProvider();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<IRepository<BlogPost>>().Should().NotBeNull();
            serviceProvider.GetService<IRepository<Skill>>().Should().NotBeNull();
        }
    }
}