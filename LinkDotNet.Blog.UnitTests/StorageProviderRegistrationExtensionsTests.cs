using System;
using FluentAssertions;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.UnitTests
{
    public class StorageProviderRegistrationExtensionsTests
    {
        [Fact]
        public void GivenAlreadyRegisteredRepository_WhenTryingToAddAnotherOne_ThenException()
        {
            var services = new ServiceCollection();
            services.UseRavenDbAsStorageProvider();

            Action act = () => services.UseSqliteAsStorageProvider();

            act.Should().Throw<NotSupportedException>();
        }
    }
}