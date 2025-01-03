using System;
using LinkDotNet.Blog.Web.RegistrationExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests;

public class StorageProviderRegistrationExtensionsTests
{
    public static TheoryData<Action<IServiceCollection>> Data => new() 
    {
        services => services.UseSqliteAsStorageProvider(),
        services => services.UseSqlAsStorageProvider(),
        services => services.UseRavenDbAsStorageProvider(),
        services => services.UseMySqlAsStorageProvider(),
        services => services.UsePostgreSqlAsStorageProvider()
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GivenAlreadyRegisteredRepository_WhenTryingToAddAnotherStorage_ThenException(Action<IServiceCollection> act)
    {
        var services = new ServiceCollection();
        services.UseRavenDbAsStorageProvider();

        Action actualAct = () => act(services);

        actualAct.ShouldThrow<NotSupportedException>();
    }
}
