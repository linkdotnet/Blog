using System;
using System.Collections.Generic;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests;

public class StorageProviderRegistrationExtensionsTests
{
    public static TheoryData<Action<IServiceCollection>> Data => new() 
    {
        services => services.UseSqliteAsStorageProvider(),
        services => services.UseSqlAsStorageProvider(),
        services => services.UseInMemoryAsStorageProvider(),
        services => services.UseRavenDbAsStorageProvider(),
        services => services.UseMySqlAsStorageProvider()
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GivenAlreadyRegisteredRepository_WhenTryingToAddAnotherStorage_ThenException(Action<IServiceCollection> act)
    {
        var services = new ServiceCollection();
        services.UseRavenDbAsStorageProvider();

        Action actualAct = () => act(services);

        actualAct.Should().Throw<NotSupportedException>();
    }
}
