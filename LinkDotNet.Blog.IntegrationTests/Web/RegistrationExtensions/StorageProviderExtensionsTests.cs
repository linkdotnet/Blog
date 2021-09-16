using System;
using System.Linq;
using FluentAssertions;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.RegistrationExtensions
{
    public class StorageProviderExtensionsTests
    {
        [Theory]
        [InlineData("SqlServer")]
        [InlineData("SqliteServer")]
        [InlineData("RavenDb")]
        [InlineData("InMemory")]
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
    }
}