using System;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests
{
    public class StorageProviderRegistrationExtensionsTests
    {
        [Fact]
        public void GivenAlreadyRegisteredRepository_WhenTryingToAddAnotherOne_ThenException()
        {
            var services = new ServiceCollection();

            services.UseSqliteAsStorageProvider();

            Assert.Throws<NotSupportedException>(() => services.UseRavenDbAsStorageProvider());
        }
    }
}